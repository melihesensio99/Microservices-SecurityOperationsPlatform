using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents;

namespace SecurityCore.Api.Features.Incidents.Create;

public sealed record CreateIncidentRequest(
    string Title,
    string Description,
    IncidentSeverity Severity,
    string? AssetName,
    string CreatedBy);

public static class CreateIncidentEndpoint
{
    public static RouteGroupBuilder MapCreateIncidentEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateIncidentAsync);
        return group;
    }

    private static async Task<IResult> CreateIncidentAsync(
        CreateIncidentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateIncidentCommand(
            request.Title,
            request.Description,
            request.Severity,
            request.AssetName,
            request.CreatedBy);

        var createdIncident = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/incidents/{createdIncident.Id}", createdIncident);
    }
}
