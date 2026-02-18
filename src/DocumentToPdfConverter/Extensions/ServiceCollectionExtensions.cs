using DocumentToPdfConverter.LibreOffice;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DocumentToPdfConverter;

/// <summary>Extension methods for registering the document converter in DI.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds DocumentToPdfConverter and LibreOffice health check to the service collection.</summary>
    public static IServiceCollection AddDocumentToPdfConverter(
        this IServiceCollection services,
        Action<LibreOfficeConverterOptions>? configure = null)
    {
        var options = new LibreOfficeConverterOptions();
        configure?.Invoke(options);

        services.AddSingleton(Options.Create(options));
        services.AddSingleton<IDocumentConverter, LibreOfficeConverter>();
        services.AddHealthChecks()
            .AddCheck<LibreOfficeHealthCheck>("libreoffice");

        return services;
    }
}
