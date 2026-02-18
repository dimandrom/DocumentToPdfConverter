namespace DocumentToPdfConverter;

/// <summary>Options for document-to-PDF conversion.</summary>
public sealed record ConversionOptions
{
    /// <summary>Override process timeout for this conversion. When null, default from converter options is used.</summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>Force landscape orientation when supported.</summary>
    public bool? Landscape { get; init; }

    /// <summary>Page range (e.g. "1-3,5") when supported by format.</summary>
    public string? PageRange { get; init; }

    /// <summary>PDF export quality.</summary>
    public PdfExportQuality Quality { get; init; } = PdfExportQuality.Default;
}

/// <summary>PDF export quality preset.</summary>
public enum PdfExportQuality : byte
{
    /// <summary>Default balance of size and quality.</summary>
    Default = 0,

    /// <summary>Higher quality, larger file.</summary>
    High,

    /// <summary>Smaller file, lower quality.</summary>
    Compressed,
}
