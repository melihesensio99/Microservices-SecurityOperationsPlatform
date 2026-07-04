using SecurityCore.Api.Domain.Incidents;

namespace SecurityCore.Api.Features.Incidents.UpdateStatus;

public sealed record UpdateIncidentStatusRequest(IncidentStatus Status);

public static class UpdateIncidentStatusEndpoint
{
    public static RouteGroupBuilder MapUpdateIncidentStatusEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/status", UpdateIncidentStatusAsync);
        return group;
    }

    private static async Task<IResult> UpdateIncidentStatusAsync(
        Guid id,
        UpdateIncidentStatusRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateIncidentStatusCommand(id, request.Status);
        var result = await sender.Send(command, cancellationToken);

        return result.ToOkHttpResult();
    }
}
