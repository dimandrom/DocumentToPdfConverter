using Microsoft.Extensions.Logging;

namespace DocumentToPdfConverter.Logging;

internal static partial class ConverterLogMessages
{
    [LoggerMessage(Level = LogLevel.Information,
        Message = "Starting conversion: {InputPath} -> {OutputPath}, type: {DocumentType}")]
    public static partial void ConversionStarted(
        this ILogger logger, string inputPath, string outputPath, DocumentType documentType);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Conversion completed in {Duration}ms, output size: {OutputSizeBytes} bytes")]
    public static partial void ConversionCompleted(
        this ILogger logger, long duration, long outputSizeBytes);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Conversion failed for {InputPath}: {ErrorMessage}")]
    public static partial void ConversionFailed(
        this ILogger logger, string inputPath, string errorMessage, Exception? ex = null);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "LibreOffice process timed out after {TimeoutMs}ms")]
    public static partial void ProcessTimedOut(
        this ILogger logger, long timeoutMs);

    [LoggerMessage(Level = LogLevel.Debug,
        Message = "Executing: {Command} {Arguments}")]
    public static partial void ProcessExecuting(
        this ILogger logger, string command, string arguments);
}
