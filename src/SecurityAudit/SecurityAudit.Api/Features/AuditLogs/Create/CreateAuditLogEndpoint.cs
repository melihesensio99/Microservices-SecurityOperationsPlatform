using SecurityPlatform.BuildingBlocks.Results;

namespace SecurityAudit.Api.Features.AuditLogs.Create;

public static class CreateAuditLogEndpoint
{
    public static RouteGroupBuilder MapCreateAuditLogEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateAuditLogAsync);
        return group;
    }

    private static async Task<IResult> CreateAuditLogAsync(
        AuditLogWriteRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateAuditLogCommand(
            request.ServiceName,
            request.Action,
            request.Level,
            request.Details,
            request.ResourceType,
            request.ResourceId,
            request.ActorId,
            request.ActorName,
            request.CorrelationId,
            request.MetadataJson,
            request.OccurredAt);

        var result = await sender.Send(command, cancellationToken);
        return result.ToCreatedHttpResult(entry => $"/api/audit-logs/{entry.Id}");
    }
}
