using System.Buffers;
using DocumentToPdfConverter.LibreOffice;

namespace DocumentToPdfConverter;

/// <summary>Converts office documents (DOC, DOCX, XLS, XLSX, ODT, ODS, PPT, PPTX) to PDF.</summary>
public interface IDocumentConverter : IAsyncDisposable
{
    /// <summary>Converts a file at <paramref name="inputPath"/> to PDF at <paramref name="outputPath"/>.</summary>
    Task<ConversionResult> ConvertAsync(
        string inputPath,
        string outputPath,
        ConversionOptions? options = null,
        CancellationToken ct = default);

    /// <summary>Converts <paramref name="input"/> stream to PDF written to <paramref name="output"/>.</summary>
    Task<ConversionResult> ConvertAsync(
        Stream input,
        Stream output,
        DocumentType documentType,
        ConversionOptions? options = null,
        CancellationToken ct = default);

    /// <summary>Converts <paramref name="inputData"/> to PDF written to <paramref name="output"/>.</summary>
    Task<ConversionResult> ConvertAsync(
        ReadOnlyMemory<byte> inputData,
        DocumentType documentType,
        IBufferWriter<byte> output,
        ConversionOptions? options = null,
        CancellationToken ct = default);

    /// <summary>Returns whether LibreOffice is available for conversion.</summary>
    Task<bool> IsAvailableAsync(CancellationToken ct = default);

    /// <summary>Returns version and path of the LibreOffice installation.</summary>
    ValueTask<LibreOfficeInfo> GetLibreOfficeInfoAsync(CancellationToken ct = default);
}
