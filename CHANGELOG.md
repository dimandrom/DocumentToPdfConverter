# Changelog
## [1.0.0]
- Initial release.
- Convert DOC, DOCX, XLS, XLSX, ODT, ODS, PPT, PPTX to PDF via LibreOffice headless.
- File, stream, and in-memory conversion overloads.
- Process pool with configurable concurrency and restart-after-N.
- DI extension and health check.
- MIT license; dependencies MIT/Apache-2.0/BSD.

## [1.0.4] - 2025-02-18
- **MS Office font mapping:** profile setup with Calibri→Carlito, Cambria→Caladea, Arial→Liberation Sans, Times New Roman→Liberation Serif, etc., so PDF export does not use DejaVu by default. Option `ApplyMsOfficeFontMapping` (default: true) in `LibreOfficeConverterOptions`; set to false to use LibreOffice defaults. For best results on Linux install `fonts-liberation`, `fonts-crosextra-carlito`, `fonts-crosextra-caladea` (Debian/Ubuntu).