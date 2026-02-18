# Использование пакета DocumentToPdfConverter / Using the DocumentToPdfConverter package

Подробное руководство по использованию NuGet-пакета после установки (`dotnet add package DocumentToPdfConverter`).

---

## Русский

### 1. Установка пакета

```bash
dotnet add package DocumentToPdfConverter
```

Или в `.csproj`:

```xml
<PackageReference Include="DocumentToPdfConverter" Version="1.0.0" />
```

### 2. Подготовка

- На машине должен быть установлен **LibreOffice** (или задана переменная `SOFFICE_PATH`). См. [INSTALLATION-LINUX](INSTALLATION-LINUX.md) для Linux.
- В коде подключите пространства имён:

```csharp
using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using Microsoft.Extensions.Options;
```

### 3. Конвертация по путям к файлам

Самый частый сценарий: входной и выходной файлы заданы путями.

```csharp
var options = new LibreOfficeConverterOptions
{
    ProcessTimeout = TimeSpan.FromSeconds(120),
    MaxConcurrency = 1,
};
await using var converter = new LibreOfficeConverter(Options.Create(options));

var result = await converter.ConvertAsync("C:/docs/report.docx", "C:/docs/report.pdf");

if (result.Success)
    Console.WriteLine($"Готово: {result.OutputSizeBytes} байт за {result.Duration.TotalSeconds:F1} с");
else
    Console.WriteLine($"Ошибка: {result.ErrorMessage}");
```

Поддерживаемые расширения: `.doc`, `.docx`, `.xls`, `.xlsx`, `.odt`, `.ods`, `.ppt`, `.pptx`. Тип определяется по расширению автоматически.

### 4. Опции конвертации

Можно передать таймаут, диапазон страниц и качество PDF:

```csharp
var conversionOptions = new ConversionOptions
{
    Timeout = TimeSpan.FromSeconds(60),
    PageRange = "1-5",
    Quality = PdfExportQuality.High,
};
var result = await converter.ConvertAsync("input.docx", "output.pdf", conversionOptions);
```

### 5. Конвертация из потока в поток

Удобно для веб-приложений: файл пришёл по HTTP, результат отдать в ответ.

```csharp
await using var inputStream = File.OpenRead("report.docx");
await using var outputStream = File.Create("report.pdf");

var result = await converter.ConvertAsync(
    inputStream,
    outputStream,
    DocumentType.Docx,
    options: null,
    cancellationToken: ct);
```

Тип документа нужно указать явно (`DocumentType.Docx`, `DocumentType.Xlsx` и т.д.), так как по потоку он не определяется.

### 6. Конвертация из памяти (ReadOnlyMemory + IBufferWriter)

Минимизация копирования при работе с уже загруженными данными:

```csharp
var inputData = await File.ReadAllBytesAsync("report.docx");
var outputWriter = new ArrayBufferWriter<byte>();

var result = await converter.ConvertAsync(
    inputData.AsMemory(),
    DocumentType.Docx,
    outputWriter,
    options: null,
    ct);

if (result.Success)
{
    var pdfBytes = outputWriter.WrittenSpan.ToArray();
    await File.WriteAllBytesAsync("report.pdf", pdfBytes, ct);
}
```

### 7. Проверка доступности LibreOffice

Перед конвертацией или для health check:

```csharp
var available = await converter.IsAvailableAsync();
if (!available)
    throw new InvalidOperationException("LibreOffice не найден. Установите его или задайте SOFFICE_PATH.");

var info = await converter.GetLibreOfficeInfoAsync();
Console.WriteLine($"LibreOffice: {info.Path}, версия: {info.Version}");
```

### 8. Регистрация в DI (ASP.NET Core и др.)

В `Program.cs` или конфигурации сервисов:

```csharp
builder.Services.AddDocumentToPdfConverter(opt =>
{
    opt.ProcessTimeout = TimeSpan.FromSeconds(120);
    opt.MaxConcurrency = 2;
    opt.SofficePath = "/usr/bin/soffice"; // необязательно, если soffice в PATH
});
```

Использование в контроллере или сервисе:

```csharp
public class ConvertController : ControllerBase
{
    private readonly IDocumentConverter _converter;

    public ConvertController(IDocumentConverter converter) => _converter = converter;

    [HttpPost("convert")]
    public async Task<IActionResult> Convert(IFormFile file, CancellationToken ct)
    {
        var type = Path.GetExtension(file.FileName).ToLowerInvariant() switch
        {
            ".docx" => DocumentType.Docx,
            ".xlsx" => DocumentType.Xlsx,
            // ... остальные
            _ => (DocumentType?)null
        };
        if (type == null) return BadRequest("Неподдерживаемый формат");

        await using var input = file.OpenReadStream();
        using var output = new MemoryStream();
        var result = await _converter.ConvertAsync(input, output, type.Value, null, ct);
        if (!result.Success) return BadRequest(result.ErrorMessage);

        output.Position = 0;
        return File(output, "application/pdf", Path.ChangeExtension(file.FileName, ".pdf"));
    }
}
```

После `AddDocumentToPdfConverter` доступен эндпоинт `/health` с проверкой LibreOffice (если настроены стандартные health checks).

### 9. Обработка ошибок и отмена

- **Несуществующий файл** → `DocumentConversionException`
- **Неподдерживаемое расширение** → `UnsupportedDocumentTypeException`
- **LibreOffice не найден** → `LibreOfficeNotFoundException`
- **Таймаут** → `ConversionTimeoutException`
- **Отмена** — передайте `CancellationToken`; при отмене получите `OperationCanceledException`

Конвертер реализует `IAsyncDisposable` — предпочтительно использовать `await using`, чтобы ресурсы пула освобождались.

---

## English

### 1. Install the package

```bash
dotnet add package DocumentToPdfConverter
```

Or in `.csproj`:

```xml
<PackageReference Include="DocumentToPdfConverter" Version="1.0.0" />
```

### 2. Prerequisites

- **LibreOffice** must be installed (or set `SOFFICE_PATH`). See [INSTALLATION-LINUX](INSTALLATION-LINUX.md) for Linux.
- Add usings:

```csharp
using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using Microsoft.Extensions.Options;
```

### 3. Convert by file paths

Most common: input and output are file paths.

```csharp
var options = new LibreOfficeConverterOptions
{
    ProcessTimeout = TimeSpan.FromSeconds(120),
    MaxConcurrency = 1,
};
await using var converter = new LibreOfficeConverter(Options.Create(options));

var result = await converter.ConvertAsync("C:/docs/report.docx", "C:/docs/report.pdf");

if (result.Success)
    Console.WriteLine($"Done: {result.OutputSizeBytes} bytes in {result.Duration.TotalSeconds:F1}s");
else
    Console.WriteLine($"Error: {result.ErrorMessage}");
```

Supported extensions: `.doc`, `.docx`, `.xls`, `.xlsx`, `.odt`, `.ods`, `.ppt`, `.pptx`. Type is detected from the extension.

### 4. Conversion options

You can pass timeout, page range, and PDF quality:

```csharp
var conversionOptions = new ConversionOptions
{
    Timeout = TimeSpan.FromSeconds(60),
    PageRange = "1-5",
    Quality = PdfExportQuality.High,
};
var result = await converter.ConvertAsync("input.docx", "output.pdf", conversionOptions);
```

### 5. Convert from stream to stream

Useful for web apps: read uploaded file, write PDF to response.

```csharp
await using var inputStream = File.OpenRead("report.docx");
await using var outputStream = File.Create("report.pdf");

var result = await converter.ConvertAsync(
    inputStream,
    outputStream,
    DocumentType.Docx,
    options: null,
    cancellationToken: ct);
```

Document type must be specified explicitly (`DocumentType.Docx`, `DocumentType.Xlsx`, etc.) because it cannot be inferred from a stream.

### 6. Convert from memory (ReadOnlyMemory + IBufferWriter)

Minimizes copying when data is already in memory:

```csharp
var inputData = await File.ReadAllBytesAsync("report.docx");
var outputWriter = new ArrayBufferWriter<byte>();

var result = await converter.ConvertAsync(
    inputData.AsMemory(),
    DocumentType.Docx,
    outputWriter,
    options: null,
    ct);

if (result.Success)
{
    var pdfBytes = outputWriter.WrittenSpan.ToArray();
    await File.WriteAllBytesAsync("report.pdf", pdfBytes, ct);
}
```

### 7. Check LibreOffice availability

Before converting or for health checks:

```csharp
var available = await converter.IsAvailableAsync();
if (!available)
    throw new InvalidOperationException("LibreOffice not found. Install it or set SOFFICE_PATH.");

var info = await converter.GetLibreOfficeInfoAsync();
Console.WriteLine($"LibreOffice: {info.Path}, version: {info.Version}");
```

### 8. Register in DI (ASP.NET Core, etc.)

In `Program.cs` or service configuration:

```csharp
builder.Services.AddDocumentToPdfConverter(opt =>
{
    opt.ProcessTimeout = TimeSpan.FromSeconds(120);
    opt.MaxConcurrency = 2;
    opt.SofficePath = "/usr/bin/soffice"; // optional if soffice is on PATH
});
```

Use in a controller or service:

```csharp
public class ConvertController : ControllerBase
{
    private readonly IDocumentConverter _converter;

    public ConvertController(IDocumentConverter converter) => _converter = converter;

    [HttpPost("convert")]
    public async Task<IActionResult> Convert(IFormFile file, CancellationToken ct)
    {
        var type = Path.GetExtension(file.FileName).ToLowerInvariant() switch
        {
            ".docx" => DocumentType.Docx,
            ".xlsx" => DocumentType.Xlsx,
            // ... others
            _ => (DocumentType?)null
        };
        if (type == null) return BadRequest("Unsupported format");

        await using var input = file.OpenReadStream();
        using var output = new MemoryStream();
        var result = await _converter.ConvertAsync(input, output, type.Value, null, ct);
        if (!result.Success) return BadRequest(result.ErrorMessage);

        output.Position = 0;
        return File(output, "application/pdf", Path.ChangeExtension(file.FileName, ".pdf"));
    }
}
```

With `AddDocumentToPdfConverter`, a health check for LibreOffice is registered; use `/health` if you have standard health checks enabled.

### 9. Error handling and cancellation

- **File not found** → `DocumentConversionException`
- **Unsupported extension** → `UnsupportedDocumentTypeException`
- **LibreOffice not found** → `LibreOfficeNotFoundException`
- **Timeout** → `ConversionTimeoutException`
- **Cancellation** — pass `CancellationToken`; on cancel you get `OperationCanceledException`

The converter implements `IAsyncDisposable` — use `await using` so pool resources are released.
