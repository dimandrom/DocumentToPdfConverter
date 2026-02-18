# Converting a document / Конвертация документа

---

## English

### Prerequisites

- [LibreOffice](https://www.libreoffice.org/download/download/) installed (Windows, Linux, or macOS).
- .NET 8 SDK: `dotnet --version` should be 8.x.

### Using the ConsoleApp sample

From the repository root:

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "<input.docx>" "<output.pdf>"
```

**Example — convert a file from Downloads:**

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.docx" "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.pdf"
```

If LibreOffice is not on `PATH`, set the environment variable (e.g. in PowerShell):

```powershell
$env:SOFFICE_PATH = "C:\Program Files\LibreOffice\program\soffice.exe"
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.docx" "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.pdf"
```

Exit codes: `0` = success, `1` = usage/input error, `2` = LibreOffice not available, `3` = conversion failed.

---

## Русский

### Что нужно

- Установленный [LibreOffice](https://www.libreoffice.org/download/download/) (Windows, Linux или macOS).
- .NET 8 SDK: `dotnet --version` должен быть 8.x.

### Запуск примера ConsoleApp

Из корня репозитория:

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "<входной.docx>" "<выходной.pdf>"
```

**Пример — конвертация файла из папки «Загрузки»:**

```bash
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.docx" "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.pdf"
```

Если LibreOffice не в `PATH`, задайте переменную окружения (например, в PowerShell):

```powershell
$env:SOFFICE_PATH = "C:\Program Files\LibreOffice\program\soffice.exe"
dotnet run --project samples/ConsoleApp/ConsoleApp.csproj -- "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.docx" "C:\Users\Dmitry\Downloads\Technical Specification Cargo Transport System.pdf"
```

Коды выхода: `0` — успех, `1` — ошибка аргументов/входа, `2` — LibreOffice недоступен, `3` — ошибка конвертации.
