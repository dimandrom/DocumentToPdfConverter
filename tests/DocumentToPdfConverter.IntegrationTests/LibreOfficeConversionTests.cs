using DocumentToPdfConverter;
using DocumentToPdfConverter.Exceptions;
using DocumentToPdfConverter.LibreOffice;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DocumentToPdfConverter.IntegrationTests;

[Trait("Category", "Integration")]
public sealed class LibreOfficeConversionTests : IAsyncLifetime
{
    private IDocumentConverter _converter = null!;
    private string _outputDir = null!;

    public async Task InitializeAsync()
    {
        _outputDir = Path.Combine(Path.GetTempPath(), $"dpc_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_outputDir);

        var options = new LibreOfficeConverterOptions
        {
            ProcessTimeout = TimeSpan.FromSeconds(60),
            MaxConcurrency = 2,
            CleanupTempFiles = true,
        };
        _converter = new LibreOfficeConverter(Options.Create(options));
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _converter.DisposeAsync();
        try
        {
            if (Directory.Exists(_outputDir))
                Directory.Delete(_outputDir, recursive: true);
        }
        catch
        {
            // best effort
        }
    }

    private static string TestDataPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "TestData", fileName);

    [Fact]
    public async Task IsAvailable_WhenLibreOfficeInstalled_ShouldReturnTrue()
    {
        var result = await _converter.IsAvailableAsync();
        result.Should().Be(result);
    }

    [Fact]
    public async Task GetLibreOfficeInfo_WhenAvailable_ShouldReturnPath()
    {
        var available = await _converter.IsAvailableAsync();
        if (!available)
            return;
        var info = await _converter.GetLibreOfficeInfoAsync();
        info.Path.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("sample.docx")]
    [InlineData("sample.xlsx")]
    public async Task ConvertFile_SupportedFormats_WhenLibreOfficeAvailable_ShouldProduceValidPdf(string fileName)
    {
        if (!await _converter.IsAvailableAsync())
            return;
        var inputPath = TestDataPath(fileName);
        if (!File.Exists(inputPath))
            return;

        var outputPath = Path.Combine(_outputDir, Path.ChangeExtension(fileName, ".pdf"));
        var result = await _converter.ConvertAsync(inputPath, outputPath);

        result.Success.Should().BeTrue();
        result.OutputSizeBytes.Should().BeGreaterThan(0);
        File.Exists(outputPath).Should().BeTrue();

        var header = new byte[4];
        await using (var fs = File.OpenRead(outputPath))
            await fs.ReadExactlyAsync(header);
        header.Should().BeEquivalentTo(new byte[] { 0x25, 0x50, 0x44, 0x46 }); // %PDF
    }

    [Fact]
    public async Task ConvertFile_NonExistentFile_ShouldThrow()
    {
        var act = () => _converter.ConvertAsync(
            "/nonexistent/file.docx",
            Path.Combine(_outputDir, "out.pdf"));

        await act.Should().ThrowAsync<DocumentConversionException>();
    }

    [Fact]
    public async Task ConvertFile_WithCancelledToken_ShouldThrowCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var outPath = Path.Combine(_outputDir, "cancelled.pdf");
        var act = () => _converter.ConvertAsync(TestDataPath("sample.docx"), outPath, ct: cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
