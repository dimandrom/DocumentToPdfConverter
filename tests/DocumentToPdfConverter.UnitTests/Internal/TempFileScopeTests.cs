using DocumentToPdfConverter.Internal;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.Internal;

public sealed class TempFileScopeTests
{
    [Fact]
    public void Create_ShouldGenerateUniquePathWithExtension()
    {
        var scope = TempFileScope.Create(".docx");
        scope.FilePath.Should().EndWith(".docx");
        scope.FilePath.Should().Contain(Path.GetTempPath());
    }

    [Fact]
    public async Task DisposeAsync_ShouldDeleteFile()
    {
        var scope = TempFileScope.Create(".tmp");
        await File.WriteAllTextAsync(scope.FilePath, "test");
        File.Exists(scope.FilePath).Should().BeTrue();

        await scope.DisposeAsync();
        File.Exists(scope.FilePath).Should().BeFalse();
    }

    [Fact]
    public async Task DisposeAsync_NonExistentFile_ShouldNotThrow()
    {
        var scope = TempFileScope.Create(".tmp");
        var act = async () => await scope.DisposeAsync();
        await act.Should().NotThrowAsync();
    }
}
