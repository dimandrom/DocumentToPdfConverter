using DocumentToPdfConverter;
using DocumentToPdfConverter.Internal;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.Internal;

public sealed class FileTypeDetectorTests
{
    [Theory]
    [InlineData("report.docx", DocumentType.Docx)]
    [InlineData("data.XLSX", DocumentType.Xlsx)]
    [InlineData("FILE.Doc", DocumentType.Doc)]
    [InlineData("sheet.ods", DocumentType.Ods)]
    [InlineData("slides.pptx", DocumentType.Pptx)]
    [InlineData("doc.doc", DocumentType.Doc)]
    [InlineData("book.odt", DocumentType.Odt)]
    [InlineData("old.xls", DocumentType.Xls)]
    [InlineData("old.ppt", DocumentType.Ppt)]
    public void Detect_ShouldReturnCorrectType(string path, DocumentType expected)
    {
        FileTypeDetector.Detect(path).Should().Be(expected);
    }

    [Theory]
    [InlineData("unknown.txt", DocumentType.Auto)]
    [InlineData("noextension", DocumentType.Auto)]
    [InlineData("", DocumentType.Auto)]
    public void Detect_UnknownOrEmpty_ShouldReturnAuto(string path, DocumentType expected)
    {
        FileTypeDetector.Detect(path).Should().Be(expected);
    }
}
