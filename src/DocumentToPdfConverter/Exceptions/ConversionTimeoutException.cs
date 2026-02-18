namespace DocumentToPdfConverter.Exceptions;

/// <summary>Thrown when conversion exceeds the allowed timeout.</summary>
public sealed class ConversionTimeoutException : DocumentConversionException
{
    /// <summary>Path to the input file that timed out.</summary>
    public string InputFile { get; }

    /// <summary>Timeout that was exceeded.</summary>
    public TimeSpan Timeout { get; }

    /// <summary>Creates an exception with input path and timeout.</summary>
    public ConversionTimeoutException(string inputFile, TimeSpan timeout)
        : base($"Conversion timed out after {timeout.TotalSeconds:F0} seconds for file: {inputFile}")
    {
        InputFile = inputFile;
        Timeout = timeout;
    }
}
