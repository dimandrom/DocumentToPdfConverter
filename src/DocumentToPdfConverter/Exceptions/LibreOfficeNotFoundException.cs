namespace DocumentToPdfConverter.Exceptions;

/// <summary>Thrown when LibreOffice (soffice) is not found. Install LibreOffice or set SOFFICE_PATH.</summary>
public sealed class LibreOfficeNotFoundException : DocumentConversionException
{
    /// <summary>Creates an exception with a message that includes install hints.</summary>
    public LibreOfficeNotFoundException()
        : base("LibreOffice (soffice) was not found. Please install LibreOffice or set the SOFFICE_PATH environment variable to the path of the soffice executable.") { }
}
