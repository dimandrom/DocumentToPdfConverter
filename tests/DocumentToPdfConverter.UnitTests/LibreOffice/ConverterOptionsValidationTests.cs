using DocumentToPdfConverter.LibreOffice;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.LibreOffice;

public sealed class ConverterOptionsValidationTests
{
    [Fact]
    public void NegativeTimeout_ShouldThrow()
    {
        var options = new LibreOfficeConverterOptions
        {
            ProcessTimeout = TimeSpan.FromSeconds(-1),
        };

        var act = () => new LibreOfficeConverter(Options.Create(options));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void InvalidMaxConcurrency_ShouldThrow(int concurrency)
    {
        var options = new LibreOfficeConverterOptions
        {
            MaxConcurrency = concurrency,
        };

        var act = () => new LibreOfficeConverter(Options.Create(options));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DefaultOptions_ShouldHaveValidDefaults()
    {
        var options = new LibreOfficeConverterOptions();
        options.ProcessTimeout.Should().BeGreaterThan(TimeSpan.Zero);
        options.MaxConcurrency.Should().BeGreaterThan(0);
        options.CleanupTempFiles.Should().BeTrue();
    }
}
