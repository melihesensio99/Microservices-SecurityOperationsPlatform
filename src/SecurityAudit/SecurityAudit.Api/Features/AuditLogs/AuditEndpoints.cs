namespace SecurityAudit.Api.Features.AuditLogs;

public static class AuditEndpoints
{
    public static RouteGroupBuilder MapAuditLogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/audit-logs");

        group.MapCreateAuditLogEndpoint();
        group.MapGetAuditLogByIdEndpoint();
        group.MapGetAuditLogsEndpoint();

        return group;
    }
}
