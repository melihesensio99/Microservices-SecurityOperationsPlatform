using SecurityCore.Api.Features.Incidents;

namespace SecurityCore.Api.Features.Incidents.AddNote;

public sealed record AddIncidentNoteRequest(
    string Author,
    string Message);

public static class AddIncidentNoteEndpoint
{
    public static RouteGroupBuilder MapAddIncidentNoteEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/notes", AddIncidentNoteAsync);
        return group;
    }

    private static async Task<IResult> AddIncidentNoteAsync(
        Guid id,
        AddIncidentNoteRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddIncidentNoteCommand(id, request.Author, request.Message);
        var incident = await sender.Send(command, cancellationToken);

        return incident is null ? Results.NotFound() : Results.Ok(incident);
    }
}
