namespace SecurityAudit.Api.Features.AuditLogs.GetList;

public static class GetAuditLogsEndpoint
{
    public static RouteGroupBuilder MapGetAuditLogsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAuditLogsAsync);
        return group;
    }

    private static async Task<IResult> GetAuditLogsAsync(
        int pageNumber,
        int pageSize,
        string? serviceName,
        AuditLogLevel? level,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditLogsQuery(new PagedRequest(pageNumber, pageSize), serviceName, level);
        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}
