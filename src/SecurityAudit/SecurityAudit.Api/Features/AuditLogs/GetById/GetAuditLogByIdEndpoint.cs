namespace SecurityAudit.Api.Features.AuditLogs.GetById;

public static class GetAuditLogByIdEndpoint
{
    public static RouteGroupBuilder MapGetAuditLogByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", GetAuditLogByIdAsync);
        return group;
    }

    private static async Task<IResult> GetAuditLogByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditLogByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.ToOkHttpResult();
    }
}
