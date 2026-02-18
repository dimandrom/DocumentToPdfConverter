using DocumentToPdfConverter;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests;

public sealed class ConversionOptionsTests
{
    [Fact]
    public void Default_Options_ShouldHaveDefaultQuality()
    {
        var options = new ConversionOptions();
        options.Quality.Should().Be(PdfExportQuality.Default);
        options.Timeout.Should().BeNull();
        options.Landscape.Should().BeNull();
        options.PageRange.Should().BeNull();
    }

    [Fact]
    public void Options_ShouldBeImmutableRecord()
    {
        var a = new ConversionOptions { Quality = PdfExportQuality.High };
        var b = new ConversionOptions { Quality = PdfExportQuality.High };
        a.Should().Be(b);
    }
}
