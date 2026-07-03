namespace SecurityCore.Api.Features.Incidents.GetById;

public static class GetIncidentByIdEndpoint
{
    public static RouteGroupBuilder MapGetIncidentByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", GetIncidentByIdAsync);
        return group;
    }

    private static async Task<IResult> GetIncidentByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var incident = await sender.Send(new GetIncidentByIdQuery(id), cancellationToken);
        return incident is null ? Results.NotFound() : Results.Ok(incident);
    }
}
