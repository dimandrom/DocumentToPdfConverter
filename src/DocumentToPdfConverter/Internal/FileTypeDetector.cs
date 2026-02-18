using System.Collections.Frozen;
using System.Runtime.CompilerServices;

namespace DocumentToPdfConverter.Internal;

internal static class FileTypeDetector
{
    private static readonly FrozenDictionary<string, DocumentType> s_extensionMap =
        new Dictionary<string, DocumentType>(StringComparer.OrdinalIgnoreCase)
        {
            [".doc"] = DocumentType.Doc,
            [".docx"] = DocumentType.Docx,
            [".xls"] = DocumentType.Xls,
            [".xlsx"] = DocumentType.Xlsx,
            [".odt"] = DocumentType.Odt,
            [".ods"] = DocumentType.Ods,
            [".ppt"] = DocumentType.Ppt,
            [".pptx"] = DocumentType.Pptx,
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DocumentType Detect(ReadOnlySpan<char> filePath)
    {
        var ext = Path.GetExtension(filePath);
        if (ext.IsEmpty)
            return DocumentType.Auto;
        return s_extensionMap.GetValueOrDefault(ext.ToString(), DocumentType.Auto);
    }
}
