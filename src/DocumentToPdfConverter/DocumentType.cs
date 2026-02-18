namespace DocumentToPdfConverter;

/// <summary>Supported document type for conversion.</summary>
public enum DocumentType : byte
{
    /// <summary>Auto-detect from file extension.</summary>
    Auto = 0,

    /// <summary>Microsoft Word 97-2003 (.doc).</summary>
    Doc,

    /// <summary>Microsoft Word Open XML (.docx).</summary>
    Docx,

    /// <summary>Microsoft Excel 97-2003 (.xls).</summary>
    Xls,

    /// <summary>Microsoft Excel Open XML (.xlsx).</summary>
    Xlsx,

    /// <summary>OpenDocument Text (.odt).</summary>
    Odt,

    /// <summary>OpenDocument Spreadsheet (.ods).</summary>
    Ods,

    /// <summary>Microsoft PowerPoint 97-2003 (.ppt).</summary>
    Ppt,

    /// <summary>Microsoft PowerPoint Open XML (.pptx).</summary>
    Pptx,
}
