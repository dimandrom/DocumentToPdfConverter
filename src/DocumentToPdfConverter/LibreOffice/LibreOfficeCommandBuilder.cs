using System.Text;

namespace DocumentToPdfConverter.LibreOffice;

internal static class LibreOfficeCommandBuilder
{
    private const string Headless = "--headless";
    private const string NoLogo = "--nologo";
    private const string NoFirstStart = "--nofirststartwizard";
    private const string ConvertToPdf = "--convert-to pdf";
    private const string OutDir = "--outdir";

    /// <summary>Builds the arguments string for soffice --headless --convert-to pdf.</summary>
    public static string BuildConvertCommand(
        string inputPath,
        string outputDir,
        ConversionOptions? options,
        string? userInstallation)
    {
        var sb = new StringBuilder(256);
        sb.Append(Headless);
        sb.Append(' ');
        sb.Append(NoLogo);
        sb.Append(' ');
        sb.Append(NoFirstStart);

        if (!string.IsNullOrEmpty(userInstallation))
        {
            sb.Append(" -env:UserInstallation=file:///");
            AppendEscapedPath(sb, userInstallation);
            sb.Append(' ');
        }

        sb.Append(' ');
        sb.Append(ConvertToPdf);

        if (options?.PageRange is { Length: > 0 } range)
        {
            sb.Append(":writer_pdf_Export:{\"PageRange\":{\"type\":\"string\",\"value\":\"");
            sb.Append(range);
            sb.Append("\"}}");
        }

        sb.Append(' ');
        sb.Append(OutDir);
        sb.Append(' ');
        AppendQuotedPath(sb, outputDir);
        sb.Append(' ');
        AppendQuotedPath(sb, inputPath);

        return sb.ToString();
    }

    private static void AppendQuotedPath(StringBuilder sb, string path)
    {
        if (path.Contains(' ') || path.Contains('"'))
        {
            sb.Append('"');
            sb.Append(path.Replace("\"", "\\\""));
            sb.Append('"');
        }
        else
        {
            sb.Append(path);
        }
    }

    private static void AppendEscapedPath(StringBuilder sb, string path)
    {
        sb.Append(path.Replace('\\', '/'));
    }
}
