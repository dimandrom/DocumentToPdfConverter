using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using DocumentToPdfConverter.Pooling;
using Microsoft.Extensions.Options;

namespace DocumentToPdfConverter.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ConversionBenchmarks
{
    private IDocumentConverter _converter = null!;
    private string _inputDocx = null!;
    private string _inputXlsx = null!;
    private string _outputDir = null!;

    [GlobalSetup]
    public void Setup()
    {
        _outputDir = Path.Combine(Path.GetTempPath(), "dpc_bench");
        Directory.CreateDirectory(_outputDir);
        _inputDocx = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.docx");
        _inputXlsx = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.xlsx");
        _converter = new LibreOfficeConverter(Options.Create(
            new LibreOfficeConverterOptions { MaxConcurrency = 1 }));
    }

    [Benchmark(Baseline = true)]
    public async Task<ConversionResult> ConvertDocx()
    {
        return await _converter.ConvertAsync(_inputDocx,
            Path.Combine(_outputDir, $"bench_{Guid.NewGuid():N}.pdf"));
    }

    [Benchmark]
    public async Task<ConversionResult> ConvertXlsx()
    {
        return await _converter.ConvertAsync(_inputXlsx,
            Path.Combine(_outputDir, $"bench_{Guid.NewGuid():N}.pdf"));
    }

    [Benchmark]
    public async Task<ConversionResult> ConvertDocxFromStream()
    {
        if (!File.Exists(_inputDocx))
            return default;
        await using var input = File.OpenRead(_inputDocx);
        using var output = StreamPoolManager.GetStream("bench");
        return await _converter.ConvertAsync(input, output, DocumentType.Docx);
    }

    [GlobalCleanup]
    public async Task Cleanup()
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
}
