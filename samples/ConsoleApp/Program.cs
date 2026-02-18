using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using Microsoft.Extensions.Options;

string? inputPath = args.Length > 0 ? args[0] : null;
if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
{
    Console.WriteLine("Usage: ConsoleApp <path-to-document>");
    Console.WriteLine("Example: ConsoleApp report.docx");
    return 1;
}

var outputPath = Path.ChangeExtension(inputPath, ".pdf");
if (args.Length > 1)
    outputPath = args[1];

var options = new LibreOfficeConverterOptions
{
    ProcessTimeout = TimeSpan.FromSeconds(120),
    MaxConcurrency = 1,
};
await using var converter = new LibreOfficeConverter(Options.Create(options));

var available = await converter.IsAvailableAsync();
if (!available)
{
    Console.WriteLine("LibreOffice is not available. Install it or set SOFFICE_PATH.");
    return 2;
}

Console.WriteLine($"Converting {inputPath} -> {outputPath}...");
var result = await converter.ConvertAsync(inputPath, outputPath);

if (result.Success)
{
    Console.WriteLine($"Done in {result.Duration.TotalSeconds:F1}s, output size: {result.OutputSizeBytes} bytes.");
    return 0;
}

Console.WriteLine($"Failed: {result.ErrorMessage}");
return 3;
