using System.Buffers;
using System.Diagnostics;

namespace DocumentToPdfConverter.Internal;

internal static class ProcessRunner
{
    private const int BufferSize = 4096;

    /// <summary>Runs a process and returns exit code. Throws on timeout or cancellation.</summary>
    public static async Task<int> RunAsync(
        string executablePath,
        string arguments,
        string? workingDirectory,
        TimeSpan timeout,
        CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = arguments,
            WorkingDirectory = workingDirectory ?? "",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        process.Start();

        var stdoutTask = ReadStreamToEndAsync(process.StandardOutput.BaseStream, cts.Token);
        var stderrTask = ReadStreamToEndAsync(process.StandardError.BaseStream, cts.Token);

        try
        {
            await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);
            await Task.WhenAll(stdoutTask, stderrTask).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested && !ct.IsCancellationRequested)
        {
            try { process.Kill(entireProcessTree: true); } catch { /* best effort */ }
            throw new TimeoutException($"Process timed out after {timeout.TotalSeconds:F0} seconds.");
        }

        return process.ExitCode;
    }

    private static async Task ReadStreamToEndAsync(Stream stream, CancellationToken ct)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            int read;
            while ((read = await stream.ReadAsync(buffer.AsMemory(0, BufferSize), ct).ConfigureAwait(false)) > 0)
            {
                // Discard output; caller can capture if needed later
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
