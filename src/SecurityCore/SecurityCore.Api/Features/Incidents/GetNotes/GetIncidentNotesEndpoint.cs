namespace SecurityCore.Api.Features.Incidents.GetNotes;

public static class GetIncidentNotesEndpoint
{
    public static RouteGroupBuilder MapGetIncidentNotesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}/notes", GetIncidentNotesAsync);
        return group;
    }

    private static async Task<IResult> GetIncidentNotesAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var notes = await sender.Send(new GetIncidentNotesQuery(id), cancellationToken);
        return notes is null ? Results.NotFound() : Results.Ok(notes);
    }
}
