namespace DocumentToPdfConverter.LibreOffice;

/// <summary>Options for LibreOffice-based conversion.</summary>
public sealed class LibreOfficeConverterOptions
{
    /// <summary>Explicit path to soffice executable. When null, auto-detect is used.</summary>
    public string? SofficePath { get; set; }

    /// <summary>Timeout for a single conversion process.</summary>
    public TimeSpan ProcessTimeout { get; set; } = TimeSpan.FromSeconds(120);

    /// <summary>Maximum concurrent conversions (separate UserInstallation per slot).</summary>
    public int MaxConcurrency { get; set; } = 1;

    /// <summary>Restart soffice after this many conversions to mitigate leaks.</summary>
    public int RestartAfterConversions { get; set; } = 50;

    /// <summary>Directory for temp files and profiles. When null, system temp is used.</summary>
    public string? TempDirectory { get; set; }

    /// <summary>Whether to delete temp files after conversion.</summary>
    public bool CleanupTempFiles { get; set; } = true;

    /// <summary>Maximum size of the conversion queue (bounded channel).</summary>
    public int MaxQueueSize { get; set; } = 100;
}
