using System.Collections.Frozen;
using System.Text;
using System.Xml;

namespace DocumentToPdfConverter.LibreOffice;

/// <summary>Creates LibreOffice user profile with MS Office → metric-compatible font mapping (avoids DejaVu).</summary>
internal static class LibreOfficeProfileSetup
{
    private const string UserSubdir = "user";
    private const string RegistryFileName = "registrymodifications.xcu";

    /// <summary>MS Office / common fonts → replacement (Liberation/Carlito/Caladea). Replacements are metric-compatible and avoid default DejaVu.</summary>
    private static readonly FrozenDictionary<string, string> DefaultFontReplacements =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Calibri"] = "Carlito",
            ["Cambria"] = "Caladea",
            ["Arial"] = "Liberation Sans",
            ["Arial Narrow"] = "Liberation Sans Narrow",
            ["Times New Roman"] = "Liberation Serif",
            ["Times"] = "Liberation Serif",
            ["Courier New"] = "Liberation Mono",
            ["Courier"] = "Liberation Mono",
            ["Verdana"] = "Liberation Sans",
            ["Georgia"] = "Liberation Serif",
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>Ensures the user profile exists and contains font replacement table so PDF export does not use DejaVu by default.</summary>
    public static void EnsureProfileWithFontMapping(string userInstallationPath)
    {
        var userDir = Path.Combine(userInstallationPath, UserSubdir);
        Directory.CreateDirectory(userDir);
        var registryPath = Path.Combine(userDir, RegistryFileName);
        var content = BuildRegistryModificationsXcu(DefaultFontReplacements);
        File.WriteAllText(registryPath, content, Encoding.UTF8);
    }

    private static string BuildRegistryModificationsXcu(IReadOnlyDictionary<string, string> fontMap)
    {
        var sb = new StringBuilder(2048);
        using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false }))
        {
            writer.WriteStartDocument(true);
            writer.WriteStartElement("oor", "items", "http://openoffice.org/2001/registry");
            writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");

            int index = 0;
            foreach (var pair in fontMap)
            {
                var path = $"/org.openoffice.Office.Common/Font/Substitution/FontPairs/Item {index}";
                index++;

                writer.WriteStartElement("item", "http://openoffice.org/2001/registry");
                writer.WriteAttributeString("oor", "path", "http://openoffice.org/2001/registry", path);
                writer.WriteAttributeString("oor", "op", "http://openoffice.org/2001/registry", "replace");

                WriteProp(writer, "Font", pair.Key);
                WriteProp(writer, "ReplaceFont", pair.Value);
                WriteProp(writer, "Always", "true");

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        return sb.ToString();
    }

    private static void WriteProp(XmlWriter writer, string name, string value)
    {
        writer.WriteStartElement("prop", "http://openoffice.org/2001/registry");
        writer.WriteAttributeString("oor", "name", "http://openoffice.org/2001/registry", name);
        writer.WriteAttributeString("oor", "op", "http://openoffice.org/2001/registry", "fuse");
        writer.WriteStartElement("value");
        writer.WriteString(value);
        writer.WriteEndElement();
        writer.WriteEndElement();
    }
}
