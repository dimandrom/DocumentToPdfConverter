using System.Runtime.InteropServices;

namespace DocumentToPdfConverter.LibreOffice;

internal sealed class LibreOfficeFinder
{
    private readonly LibreOfficeConverterOptions _options;

    public LibreOfficeFinder(LibreOfficeConverterOptions options)
    {
        _options = options;
    }

    /// <summary>Returns the configured path if set; otherwise null.</summary>
    public string? GetConfiguredPath()
    {
        if (!string.IsNullOrWhiteSpace(_options.SofficePath))
            return _options.SofficePath!.Trim();
        var env = Environment.GetEnvironmentVariable("SOFFICE_PATH");
        return string.IsNullOrWhiteSpace(env) ? null : env!.Trim();
    }

    /// <summary>Returns the path to use for soffice, or throws if not found.</summary>
    public string Find()
    {
        var configured = GetConfiguredPath();
        if (!string.IsNullOrEmpty(configured) && File.Exists(configured))
            return configured!;

        foreach (var candidate in GetSearchPaths())
        {
            if (!string.IsNullOrEmpty(candidate) && File.Exists(candidate))
                return candidate;
        }

        throw new Exceptions.LibreOfficeNotFoundException();
    }

    /// <summary>Returns candidate paths for discovery (for tests and diagnostics).</summary>
    public IEnumerable<string> GetSearchPaths()
    {
        var configured = GetConfiguredPath();
        if (!string.IsNullOrEmpty(configured))
            yield return configured!;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var pfx = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            yield return Path.Combine(pf, "LibreOffice", "program", "soffice.exe");
            yield return Path.Combine(pfx, "LibreOffice", "program", "soffice.exe");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            yield return "/usr/bin/soffice";
            yield return "/usr/bin/libreoffice";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            yield return "/Applications/LibreOffice.app/Contents/MacOS/soffice";
        }
    }
}
