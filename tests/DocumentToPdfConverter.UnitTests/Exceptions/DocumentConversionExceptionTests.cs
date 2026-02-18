using DocumentToPdfConverter.Exceptions;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.Exceptions;

public sealed class DocumentConversionExceptionTests
{
    [Fact]
    public void LibreOfficeNotFound_ShouldContainInstallHint()
    {
        var ex = new LibreOfficeNotFoundException();
        ex.Message.Should().Contain("LibreOffice");
        ex.Message.Should().Contain("install");
    }

    [Fact]
    public void TimeoutException_ShouldContainDuration()
    {
        var ex = new ConversionTimeoutException("/tmp/file.docx", TimeSpan.FromSeconds(30));
        ex.Message.Should().Contain("30");
        ex.InputFile.Should().Be("/tmp/file.docx");
    }
}
