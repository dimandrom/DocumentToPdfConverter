using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DocumentToPdfConverter.LibreOffice;

/// <summary>Health check that verifies LibreOffice is available.</summary>
public sealed class LibreOfficeHealthCheck : IHealthCheck
{
    private readonly IDocumentConverter _converter;

    public LibreOfficeHealthCheck(IDocumentConverter converter)
    {
        _converter = converter;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var available = await _converter.IsAvailableAsync(cancellationToken).ConfigureAwait(false);
            return available
                ? HealthCheckResult.Healthy("LibreOffice is available.")
                : HealthCheckResult.Unhealthy("LibreOffice is not available.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("LibreOffice check failed.", ex);
        }
    }
}
