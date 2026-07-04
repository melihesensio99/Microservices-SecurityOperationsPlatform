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
        var result = await sender.Send(new GetIncidentByIdQuery(id), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Json(
                new { error = result.Error.Message, code = result.Error.Code },
                statusCode: result.Error.StatusCode);
    }
}
