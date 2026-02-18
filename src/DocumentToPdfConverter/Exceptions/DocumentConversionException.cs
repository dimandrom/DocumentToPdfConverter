namespace DocumentToPdfConverter.Exceptions;

/// <summary>Thrown when document conversion fails.</summary>
public class DocumentConversionException : Exception
{
    /// <summary>Creates an exception with the given message.</summary>
    public DocumentConversionException(string message) : base(message) { }

    /// <summary>Creates an exception with the given message and inner exception.</summary>
    public DocumentConversionException(string message, Exception inner)
        : base(message, inner) { }
}
