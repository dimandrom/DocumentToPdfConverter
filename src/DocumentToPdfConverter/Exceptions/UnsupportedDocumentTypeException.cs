namespace DocumentToPdfConverter.Exceptions;

/// <summary>Thrown when the document type or file extension is not supported for conversion.</summary>
public sealed class UnsupportedDocumentTypeException : DocumentConversionException
{
    /// <summary>Creates an exception with the given message.</summary>
    public UnsupportedDocumentTypeException(string message) : base(message) { }
}
