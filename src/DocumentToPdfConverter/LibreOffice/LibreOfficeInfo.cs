namespace DocumentToPdfConverter.LibreOffice;

/// <summary>Information about the detected LibreOffice installation.</summary>
/// <param name="Version">Version string (e.g. from soffice --version).</param>
/// <param name="Path">Full path to the soffice executable.</param>
public sealed record LibreOfficeInfo(string Version, string Path);
