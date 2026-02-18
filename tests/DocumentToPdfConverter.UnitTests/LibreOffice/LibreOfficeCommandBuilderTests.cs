using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.LibreOffice;

public sealed class LibreOfficeCommandBuilderTests
{
    [Fact]
    public void BuildConvertCommand_ShouldContainHeadlessFlag()
    {
        var args = LibreOfficeCommandBuilder.BuildConvertCommand(
            "/tmp/input.docx", "/tmp/output/", null, null);
        args.Should().Contain("--headless");
    }

    [Fact]
    public void BuildConvertCommand_ShouldContainConvertToPdf()
    {
        var args = LibreOfficeCommandBuilder.BuildConvertCommand(
            "/tmp/input.docx", "/tmp/output/", null, null);
        args.Should().Contain("--convert-to pdf");
    }

    [Fact]
    public void BuildConvertCommand_ShouldContainOutdir()
    {
        var args = LibreOfficeCommandBuilder.BuildConvertCommand(
            "/tmp/input.docx", "/tmp/output/", null, null);
        args.Should().Contain("--outdir");
        args.Should().Contain("/tmp/output/");
    }

    [Fact]
    public void BuildConvertCommand_ShouldContainInputPath()
    {
        var args = LibreOfficeCommandBuilder.BuildConvertCommand(
            "/tmp/input.docx", "/tmp/output/", null, null);
        args.Should().Contain("/tmp/input.docx");
    }
}
