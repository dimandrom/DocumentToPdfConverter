# Troubleshooting / Решение проблем

---

## English

### LibreOffice not found

- **Install LibreOffice** on the server or machine where the converter runs. The package uses the `soffice` command-line tool.  
  **Linux:** see [INSTALLATION-LINUX](INSTALLATION-LINUX.md) (Ubuntu/Debian, RHEL, server group).
- **Set `SOFFICE_PATH`** to the full path of the `soffice` executable if it is not on `PATH`.
- **Typical paths:**
  - Windows: `C:\Program Files\LibreOffice\program\soffice.exe`
  - Linux: `/usr/bin/soffice` or `/usr/bin/libreoffice`
  - macOS: `/Applications/LibreOffice.app/Contents/MacOS/soffice`
- When using **LibreOfficeConverterOptions**, set **SofficePath** explicitly if auto-discovery fails.

### Timeouts

- Increase **ProcessTimeout** in **LibreOfficeConverterOptions** (default 120 seconds).
- For a single conversion, pass **ConversionOptions** with **Timeout**.
- Large or complex documents may need longer timeouts.

### Temporary files and permissions

- The converter writes temporary files under the system temp directory (or **TempDirectory** in options). Ensure the process has read/write access.
- Set **CleanupTempFiles** to `true` (default) so temp files are removed after conversion.

### Concurrency and restarts

- **MaxConcurrency**: LibreOffice does not support multiple conversions in one profile. This package uses separate UserInstallation directories per slot. Keep **MaxConcurrency** low (e.g. 1–4) to avoid high memory usage.
- **RestartAfterConversions**: After this many conversions, the pool can restart processes to reduce memory leaks. Default is 50.

### Health check

- When using **AddDocumentToPdfConverter**, a health check named `"libreoffice"` is registered. Use `/health` (or your health URL) to verify that LibreOffice is available.

### Fonts in PDF (DejaVu vs MS Office–like)

By default LibreOffice substitutes missing fonts (e.g. Calibri, Cambria) with DejaVu, which can look larger or different. This package applies an **MS Office font mapping** in the LibreOffice profile: Calibri→Carlito, Cambria→Caladea, Arial→Liberation Sans, Times New Roman→Liberation Serif, etc., so PDFs are closer to the original layout. It is enabled by default (**ApplyMsOfficeFontMapping** = true in **LibreOfficeConverterOptions**). Set it to **false** to use LibreOffice’s default (e.g. DejaVu). For best results on Linux install metric-compatible fonts: `fonts-liberation`, `fonts-carlito`, `fonts-caladea` (or `liberation-fonts`, `ttf-carlito`, `ttf-caladea` depending on distro). See [INSTALLATION-LINUX](INSTALLATION-LINUX.md).

---

## Русский

### LibreOffice не найден

- **Установите LibreOffice** на сервере или машине, где запускается конвертер. Библиотека использует консольную утилиту `soffice`.  
  **Linux:** см. [INSTALLATION-LINUX](INSTALLATION-LINUX.md) (Ubuntu/Debian, RHEL, группа серверов).
- **Задайте переменную окружения `SOFFICE_PATH`** полным путём к исполняемому файлу `soffice`, если он не в `PATH`.
- **Типичные пути:**
  - Windows: `C:\Program Files\LibreOffice\program\soffice.exe`
  - Linux: `/usr/bin/soffice` или `/usr/bin/libreoffice`
  - macOS: `/Applications/LibreOffice.app/Contents/MacOS/soffice`
- В **LibreOfficeConverterOptions** при необходимости укажите **SofficePath** вручную.

### Таймауты

- Увеличьте **ProcessTimeout** в **LibreOfficeConverterOptions** (по умолчанию 120 секунд).
- Для одной конвертации можно передать **ConversionOptions** с **Timeout**.
- Крупные или сложные документы могут требовать больше времени.

### Временные файлы и права доступа

- Конвертер создаёт временные файлы в системной папке для временных файлов (или в **TempDirectory** в настройках). Убедитесь, что у процесса есть права на чтение и запись.
- **CleanupTempFiles** = `true` (по умолчанию) — временные файлы удаляются после конвертации.

### Конкурентность и перезапуск

- **MaxConcurrency**: LibreOffice не поддерживает несколько конвертаций в одном профиле. В пакете для каждого слота используется отдельная папка UserInstallation. Рекомендуется держать **MaxConcurrency** небольшим (1–4), чтобы не перегружать память.
- **RestartAfterConversions**: после указанного числа конвертаций пул может перезапускать процессы, чтобы снизить утечки памяти. По умолчанию 50.

### Проверка здоровья (health check)

- При использовании **AddDocumentToPdfConverter** регистрируется проверка `"libreoffice"`. Эндпоинт `/health` (или ваш URL проверки) позволяет убедиться, что LibreOffice доступен.

### Шрифты в PDF (DejaVu и подстановка под MS Office)

По умолчанию LibreOffice подставляет отсутствующие шрифты (Calibri, Cambria и т.д.) шрифтом DejaVu, из‑за чего PDF может отличаться по виду. В этом пакете в профиле LibreOffice задаётся **маппинг шрифтов MS Office**: Calibri→Carlito, Cambria→Caladea, Arial→Liberation Sans, Times New Roman→Liberation Serif и др., чтобы PDF был ближе к оригиналу. Включено по умолчанию (**ApplyMsOfficeFontMapping** = true в **LibreOfficeConverterOptions**). Установите **false**, чтобы использовать подстановку по умолчанию (например DejaVu). Для лучшего результата на Linux установите метрически совместимые шрифты: `fonts-liberation`, `fonts-carlito`, `fonts-caladea` (или `liberation-fonts`, `ttf-carlito`, `ttf-caladea` в зависимости от дистрибутива). См. [INSTALLATION-LINUX](INSTALLATION-LINUX.md).
