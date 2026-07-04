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
        var result = await sender.Send(new GetIncidentNotesQuery(id), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Json(
                new { error = result.Error.Message, code = result.Error.Code },
                statusCode: result.Error.StatusCode);
    }
}
