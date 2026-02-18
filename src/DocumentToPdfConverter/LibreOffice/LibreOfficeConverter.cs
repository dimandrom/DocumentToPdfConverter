using System.Buffers;
using System.Diagnostics;
using DocumentToPdfConverter.Exceptions;
using DocumentToPdfConverter.Internal;
using DocumentToPdfConverter.Logging;
using DocumentToPdfConverter.Pooling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocumentToPdfConverter.LibreOffice;

/// <summary>Converts documents to PDF using LibreOffice headless.</summary>
public sealed class LibreOfficeConverter : IDocumentConverter
{
    private readonly LibreOfficeConverterOptions _options;
    private readonly LibreOfficeFinder _finder;
    private readonly LibreOfficeProcessPool _pool;
    private readonly ILogger? _logger;

    public LibreOfficeConverter(IOptions<LibreOfficeConverterOptions> options)
        : this(options, logger: null) { }

    public LibreOfficeConverter(IOptions<LibreOfficeConverterOptions> options, ILogger? logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        if (_options.ProcessTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options), "ProcessTimeout must be positive.");
        if (_options.MaxConcurrency < 1)
            throw new ArgumentOutOfRangeException(nameof(options), "MaxConcurrency must be at least 1.");
        _finder = new LibreOfficeFinder(_options);
        _logger = logger;
        _pool = new LibreOfficeProcessPool(_options, _finder, _logger);
    }

    /// <inheritdoc />
    public async Task<ConversionResult> ConvertAsync(
        string inputPath,
        string outputPath,
        ConversionOptions? options = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new ArgumentNullException(nameof(inputPath));
        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentNullException(nameof(outputPath));

        var documentType = FileTypeDetector.Detect(inputPath);
        if (documentType == DocumentType.Auto)
            throw new UnsupportedDocumentTypeException($"Unsupported or missing file extension: {inputPath}");

        if (!File.Exists(inputPath))
            throw new DocumentConversionException($"Input file not found: {inputPath}");

        var outputDir = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrEmpty(outputDir))
            throw new ArgumentException("Output path must contain a directory.", nameof(outputPath));
        Directory.CreateDirectory(outputDir);

        var timeout = options?.Timeout ?? _options.ProcessTimeout;
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options), "Timeout must be positive.");

        _logger?.ConversionStarted(inputPath, outputPath, documentType);
        var sw = Stopwatch.StartNew();

        try
        {
            var result = await _pool.ConvertAsync(inputPath, outputDir, options, timeout, ct).ConfigureAwait(false);
            sw.Stop();
            if (result.Success)
                _logger?.ConversionCompleted(sw.ElapsedMilliseconds, result.OutputSizeBytes);
            else
                _logger?.ConversionFailed(inputPath, result.ErrorMessage ?? "Unknown error");
            return result with { Duration = sw.Elapsed };
        }
        catch (TimeoutException)
        {
            sw.Stop();
            throw new ConversionTimeoutException(inputPath, timeout);
        }
    }

    /// <inheritdoc />
    public async Task<ConversionResult> ConvertAsync(
        Stream input,
        Stream output,
        DocumentType documentType,
        ConversionOptions? options = null,
        CancellationToken ct = default)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        if (output == null)
            throw new ArgumentNullException(nameof(output));
        if (documentType == DocumentType.Auto)
            throw new ArgumentException("DocumentType must be specified when using streams.", nameof(documentType));

        var ext = GetExtension(documentType);
        await using var inputTemp = TempFileScope.Create(ext);
        var outputDir = Path.GetDirectoryName(inputTemp.FilePath)!;
        using (var fs = new FileStream(inputTemp.FilePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920,
                   FileOptions.Asynchronous))
        {
            await StreamCopyHelper.CopyToAsync(input, fs, ct).ConfigureAwait(false);
        }

        var timeout = options?.Timeout ?? _options.ProcessTimeout;
        var result = await _pool.ConvertAsync(inputTemp.FilePath, outputDir, options, timeout, ct).ConfigureAwait(false);
        if (!result.Success)
            return result;

        var pdfPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(inputTemp.FilePath) + ".pdf");
        if (!File.Exists(pdfPath))
            return result with { Success = false, ErrorMessage = "PDF was not produced." };

        await using (var pdfStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan))
        {
            await StreamCopyHelper.CopyToAsync(pdfStream, output, ct).ConfigureAwait(false);
        }

        if (_options.CleanupTempFiles)
        {
            try { File.Delete(pdfPath); } catch { /* best effort */ }
        }

        return new ConversionResult
        {
            Success = true,
            OutputSizeBytes = result.OutputSizeBytes,
            Duration = result.Duration,
        };
    }

    /// <inheritdoc />
    public async Task<ConversionResult> ConvertAsync(
        ReadOnlyMemory<byte> inputData,
        DocumentType documentType,
        IBufferWriter<byte> output,
        ConversionOptions? options = null,
        CancellationToken ct = default)
    {
        if (documentType == DocumentType.Auto)
            throw new ArgumentException("DocumentType must be specified.", nameof(documentType));
        if (output == null)
            throw new ArgumentNullException(nameof(output));

        var ext = GetExtension(documentType);
        await using var inputTemp = TempFileScope.Create(ext);
        await File.WriteAllBytesAsync(inputTemp.FilePath, inputData.ToArray(), ct).ConfigureAwait(false);

        var outputDir = Path.GetDirectoryName(inputTemp.FilePath)!;
        var timeout = options?.Timeout ?? _options.ProcessTimeout;
        var result = await _pool.ConvertAsync(inputTemp.FilePath, outputDir, options, timeout, ct).ConfigureAwait(false);
        if (!result.Success)
            return result;

        var pdfPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(inputTemp.FilePath) + ".pdf");
        if (!File.Exists(pdfPath))
            return result with { Success = false, ErrorMessage = "PDF was not produced." };

        var pdfBytes = await File.ReadAllBytesAsync(pdfPath, ct).ConfigureAwait(false);
        output.Write(pdfBytes.AsSpan());
        if (_options.CleanupTempFiles)
        {
            try { File.Delete(pdfPath); } catch { /* best effort */ }
        }

        return new ConversionResult
        {
            Success = true,
            OutputSizeBytes = pdfBytes.Length,
            Duration = result.Duration,
        };
    }

    /// <inheritdoc />
    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var path = _finder.Find();
            if (string.IsNullOrEmpty(path))
                return false;
            return await Task.FromResult(File.Exists(path)).ConfigureAwait(false);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<LibreOfficeInfo> GetLibreOfficeInfoAsync(CancellationToken ct = default)
    {
        var path = _finder.Find();
        var version = "unknown";
        try
        {
            var exitCode = await ProcessRunner.RunAsync(path, "--version", null, TimeSpan.FromSeconds(5), ct).ConfigureAwait(false);
            // Version is on stdout; we don't capture it in ProcessRunner. For now use placeholder.
        }
        catch
        {
            // ignore
        }

        return await ValueTask.FromResult(new LibreOfficeInfo(version, path)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _pool.DisposeAsync().ConfigureAwait(false);

    private static string GetExtension(DocumentType type)
    {
        return type switch
        {
            DocumentType.Doc => ".doc",
            DocumentType.Docx => ".docx",
            DocumentType.Xls => ".xls",
            DocumentType.Xlsx => ".xlsx",
            DocumentType.Odt => ".odt",
            DocumentType.Ods => ".ods",
            DocumentType.Ppt => ".ppt",
            DocumentType.Pptx => ".pptx",
            _ => ".bin",
        };
    }
}
