using DocumentToPdfConverter;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests;

public sealed class ConversionResultTests
{
    [Fact]
    public void SuccessResult_ShouldBeValueType()
    {
        typeof(ConversionResult).IsValueType.Should().BeTrue();
    }

    [Fact]
    public void SuccessResult_ShouldHaveCorrectValues()
    {
        var result = new ConversionResult
        {
            Success = true,
            OutputPath = "/tmp/out.pdf",
            OutputSizeBytes = 1024,
            Duration = TimeSpan.FromMilliseconds(500),
        };

        result.Success.Should().BeTrue();
        result.OutputSizeBytes.Should().Be(1024);
        result.OutputPath.Should().Be("/tmp/out.pdf");
        result.Duration.Should().Be(TimeSpan.FromMilliseconds(500));
        result.ErrorMessage.Should().BeNull();
    }
}
