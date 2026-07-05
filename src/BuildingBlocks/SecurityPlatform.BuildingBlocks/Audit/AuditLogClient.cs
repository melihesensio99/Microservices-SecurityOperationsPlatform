using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SecurityPlatform.BuildingBlocks.Audit;

public sealed class AuditLogClient : IAuditLogClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditLogClient> _logger;

    public AuditLogClient(HttpClient httpClient, ILogger<AuditLogClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> TryWriteAsync(AuditLogWriteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("/api/audit-logs", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            _logger.LogWarning(
                "Audit service returned {StatusCode} while writing {Action} for {ServiceName}",
                response.StatusCode,
                request.Action,
                request.ServiceName);
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to write audit log {Action} for {ServiceName}",
                request.Action,
                request.ServiceName);
            return false;
        }
    }
}
