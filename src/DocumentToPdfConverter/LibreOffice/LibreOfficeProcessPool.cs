using System.Diagnostics;
using System.Threading.Channels;
using DocumentToPdfConverter.Internal;
using DocumentToPdfConverter.Logging;
using Microsoft.Extensions.Logging;

namespace DocumentToPdfConverter.LibreOffice;

internal sealed class LibreOfficeProcessPool : IAsyncDisposable
{
    private readonly LibreOfficeConverterOptions _options;
    private readonly LibreOfficeFinder _finder;
    private readonly ILogger? _logger;
    private readonly SemaphoreSlim _semaphore;
    private readonly Channel<ConversionWorkItem> _channel;
    private readonly string _tempBase;
    private volatile bool _disposed;
    private readonly Task _workerTask;

    public LibreOfficeProcessPool(
        LibreOfficeConverterOptions options,
        LibreOfficeFinder finder,
        ILogger? logger)
    {
        _options = options;
        _finder = finder;
        _logger = logger;
        _semaphore = new SemaphoreSlim(options.MaxConcurrency, options.MaxConcurrency);
        _channel = Channel.CreateBounded<ConversionWorkItem>(
            new BoundedChannelOptions(options.MaxQueueSize) { FullMode = BoundedChannelFullMode.Wait });
        _tempBase = string.IsNullOrEmpty(options.TempDirectory)
            ? Path.Combine(Path.GetTempPath(), "dpc_pool")
            : Path.Combine(options.TempDirectory, "dpc_pool");
        _workerTask = RunWorkerAsync();
    }

    public async Task<ConversionResult> ConvertAsync(
        string inputPath,
        string outputDir,
        ConversionOptions? conversionOptions,
        TimeSpan timeout,
        CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<ConversionResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var item = new ConversionWorkItem(inputPath, outputDir, conversionOptions, timeout, tcs, ct);
        await _channel.Writer.WriteAsync(item, ct).ConfigureAwait(false);
        return await tcs.Task.ConfigureAwait(false);
    }

    private async Task RunWorkerAsync()
    {
        await foreach (var item in _channel.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            if (_disposed)
            {
                item.Complete(new ConversionResult
                {
                    Success = false,
                    ErrorMessage = "Converter was disposed.",
                    Duration = TimeSpan.Zero,
                });
                continue;
            }

            await _semaphore.WaitAsync(item.CancellationToken).ConfigureAwait(false);
            _ = ExecuteOneAsync(item);
        }
    }

    private async Task ExecuteOneAsync(ConversionWorkItem item)
    {
        try
        {
            var slotIndex = GetNextSlotIndex();
            var userInstallation = Path.Combine(_tempBase, slotIndex.ToString());
            Directory.CreateDirectory(userInstallation);

            var sofficePath = _finder.Find();
            var args = LibreOfficeCommandBuilder.BuildConvertCommand(
                item.InputPath,
                item.OutputDir,
                item.ConversionOptions,
                userInstallation);

            _logger?.ProcessExecuting(sofficePath, args);

            var sw = Stopwatch.StartNew();
            var exitCode = await ProcessRunner.RunAsync(
                sofficePath,
                args,
                null,
                item.Timeout,
                item.CancellationToken).ConfigureAwait(false);
            sw.Stop();

            var outputPath = Path.Combine(item.OutputDir, Path.GetFileNameWithoutExtension(item.InputPath) + ".pdf");
            long size = 0;
            if (File.Exists(outputPath))
            {
                var fi = new FileInfo(outputPath);
                size = fi.Length;
            }

            item.Complete(new ConversionResult
            {
                Success = exitCode == 0 && size > 0,
                OutputPath = outputPath,
                OutputSizeBytes = size,
                Duration = sw.Elapsed,
                ErrorMessage = exitCode != 0 ? $"soffice exited with code {exitCode}" : null,
            });
        }
        catch (OperationCanceledException)
        {
            item.Complete(new ConversionResult
            {
                Success = false,
                ErrorMessage = "Conversion was cancelled.",
                Duration = TimeSpan.Zero,
            });
        }
        catch (TimeoutException ex)
        {
            _logger?.ProcessTimedOut((long)item.Timeout.TotalMilliseconds);
            item.Complete(new ConversionResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Duration = item.Timeout,
            });
        }
        catch (Exception ex)
        {
            item.Complete(new ConversionResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Duration = TimeSpan.Zero,
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private int _slotCounter;
    private int GetNextSlotIndex() => Interlocked.Increment(ref _slotCounter) % Math.Max(1, _options.MaxConcurrency);

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        _disposed = true;
        _channel.Writer.Complete();
        await _workerTask.ConfigureAwait(false);
        _semaphore.Dispose();
        try
        {
            if (Directory.Exists(_tempBase))
                Directory.Delete(_tempBase, recursive: true);
        }
        catch
        {
            // Best-effort cleanup
        }
    }

    private readonly struct ConversionWorkItem
    {
        public string InputPath { get; }
        public string OutputDir { get; }
        public ConversionOptions? ConversionOptions { get; }
        public TimeSpan Timeout { get; }
        public CancellationToken CancellationToken { get; }
        private readonly TaskCompletionSource<ConversionResult> _tcs;

        public ConversionWorkItem(
            string inputPath,
            string outputDir,
            ConversionOptions? conversionOptions,
            TimeSpan timeout,
            TaskCompletionSource<ConversionResult> tcs,
            CancellationToken ct)
        {
            InputPath = inputPath;
            OutputDir = outputDir;
            ConversionOptions = conversionOptions;
            Timeout = timeout;
            _tcs = tcs;
            CancellationToken = ct;
        }

        public void Complete(ConversionResult result) => _tcs.TrySetResult(result);
    }
}
