using DocumentToPdfConverter.LibreOffice;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.LibreOffice;

public sealed class LibreOfficeFinderTests
{
    [Fact]
    public void GetConfiguredPath_WithExplicitPath_ShouldReturnPath()
    {
        var options = new LibreOfficeConverterOptions
        {
            SofficePath = "/usr/bin/soffice",
        };

        var finder = new LibreOfficeFinder(options);
        var path = finder.GetConfiguredPath();
        path.Should().Be("/usr/bin/soffice");
    }

    [Fact]
    public void GetSearchPaths_WithEnvironmentVariable_ShouldContainEnvPath()
    {
        Environment.SetEnvironmentVariable("SOFFICE_PATH", "/test/soffice");
        try
        {
            var finder = new LibreOfficeFinder(new LibreOfficeConverterOptions());
            var candidates = finder.GetSearchPaths().ToList();
            candidates.Should().Contain("/test/soffice");
        }
        finally
        {
            Environment.SetEnvironmentVariable("SOFFICE_PATH", null);
        }
    }
}
