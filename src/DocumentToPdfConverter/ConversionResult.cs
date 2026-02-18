namespace DocumentToPdfConverter;

/// <summary>Result of a single conversion operation.</summary>
public readonly record struct ConversionResult
{
    /// <summary>Whether the conversion succeeded.</summary>
    public required bool Success { get; init; }

    /// <summary>Path to the output PDF file when conversion was file-based; otherwise null.</summary>
    public string? OutputPath { get; init; }

    /// <summary>Size of the output in bytes.</summary>
    public long OutputSizeBytes { get; init; }

    /// <summary>Duration of the conversion.</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>Error message when conversion failed.</summary>
    public string? ErrorMessage { get; init; }
}
