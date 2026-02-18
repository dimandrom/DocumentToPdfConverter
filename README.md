# DocumentToPdfConverter

High-performance open-source .NET library for converting office documents to PDF using LibreOffice headless. Supports DOC, DOCX, XLS, XLSX, ODT, ODS, PPT, PPTX.

**License:** MIT. Dependencies are MIT/Apache-2.0/BSD compatible. No proprietary libraries (Aspose, IronPDF, Syncfusion, Spire, etc.).

---

## English

### Requirements

- .NET 8+
- LibreOffice installed and `soffice` available on the system (or set `SOFFICE_PATH`).  
  **Linux (server/workstations):** see [INSTALLATION-LINUX](docs/INSTALLATION-LINUX.md).

### Installation

```bash
dotnet add package DocumentToPdfConverter
```

### Quick start

```csharp
using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using Microsoft.Extensions.Options;

var options = new LibreOfficeConverterOptions
{
    ProcessTimeout = TimeSpan.FromSeconds(120),
    MaxConcurrency = 1,
};
await using var converter = new LibreOfficeConverter(Options.Create(options));

var result = await converter.ConvertAsync("input.docx", "output.pdf");
if (result.Success)
    Console.WriteLine($"Converted in {result.Duration.TotalSeconds:F1}s, size: {result.OutputSizeBytes} bytes");
```

### Converting a file from command line (sample)

With LibreOffice installed, you can use the ConsoleApp sample:

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "path/to/document.docx" "path/to/output.pdf"
```

Example:

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "C:\Users\YourName\Downloads\Technical Specification.docx" "C:\Users\YourName\Downloads\Technical Specification.pdf"
```

### Dependency injection

```csharp
services.AddDocumentToPdfConverter(opt =>
{
    opt.ProcessTimeout = TimeSpan.FromSeconds(120);
    opt.MaxConcurrency = 2;
});
// Registers IDocumentConverter and a health check named "libreoffice"
```

### API overview

- **ConvertAsync(path, path)** – convert a file to a PDF file.
- **ConvertAsync(stream, stream, documentType)** – convert from/to streams.
- **ConvertAsync(ReadOnlyMemory&lt;byte&gt;, documentType, IBufferWriter&lt;byte&gt;)** – convert in-memory.
- **IsAvailableAsync()** – check if LibreOffice is available.
- **GetLibreOfficeInfoAsync()** – get version and path of the detected installation.

See [TROUBLESHOOTING](docs/TROUBLESHOOTING.md) for common issues and configuration tips.  
[Install LibreOffice on Linux](docs/INSTALLATION-LINUX.md) (Ubuntu/Debian, RHEL, server group).

---

## Русский

### Требования

- .NET 8+
- Установленный LibreOffice, в системе доступна команда `soffice` (или задана переменная окружения `SOFFICE_PATH`).  
  **Linux (сервер/рабочие станции):** см. [INSTALLATION-LINUX](docs/INSTALLATION-LINUX.md).

### Установка

```bash
dotnet add package DocumentToPdfConverter
```

### Быстрый старт

```csharp
using DocumentToPdfConverter;
using DocumentToPdfConverter.LibreOffice;
using Microsoft.Extensions.Options;

var options = new LibreOfficeConverterOptions
{
    ProcessTimeout = TimeSpan.FromSeconds(120),
    MaxConcurrency = 1,
};
await using var converter = new LibreOfficeConverter(Options.Create(options));

var result = await converter.ConvertAsync("input.docx", "output.pdf");
if (result.Success)
    Console.WriteLine($"Конвертировано за {result.Duration.TotalSeconds:F1} с, размер: {result.OutputSizeBytes} байт");
```

### Конвертация файла из командной строки (пример)

При установленном LibreOffice можно использовать пример ConsoleApp:

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "путь/к/документу.docx" "путь/к/результату.pdf"
```

Пример:

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "C:\Users\Имя\Downloads\Техническая спецификация.docx" "C:\Users\Имя\Downloads\Техническая спецификация.pdf"
```

### Регистрация в DI

```csharp
services.AddDocumentToPdfConverter(opt =>
{
    opt.ProcessTimeout = TimeSpan.FromSeconds(120);
    opt.MaxConcurrency = 2;
});
// Регистрирует IDocumentConverter и проверку здоровья "libreoffice"
```

### Обзор API

- **ConvertAsync(path, path)** — конвертация файла в PDF.
- **ConvertAsync(stream, stream, documentType)** — конвертация из потока в поток.
- **ConvertAsync(ReadOnlyMemory&lt;byte&gt;, documentType, IBufferWriter&lt;byte&gt;)** — конвертация в памяти.
- **IsAvailableAsync()** — проверка доступности LibreOffice.
- **GetLibreOfficeInfoAsync()** — получение версии и пути к установленному LibreOffice.

Подробнее о типичных проблемах и настройке: [TROUBLESHOOTING](docs/TROUBLESHOOTING.md).  
[Установка LibreOffice на Linux](docs/INSTALLATION-LINUX.md) (Ubuntu/Debian, RHEL, группа серверов).
