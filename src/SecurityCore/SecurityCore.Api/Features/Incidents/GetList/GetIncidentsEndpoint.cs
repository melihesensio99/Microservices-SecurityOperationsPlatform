namespace SecurityCore.Api.Features.Incidents.GetList;

public static class GetIncidentsEndpoint
{
    public static RouteGroupBuilder MapGetIncidentsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetIncidentsAsync);
        return group;
    }

    private static async Task<IResult> GetIncidentsAsync(
        ISender sender,
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var incidents = await sender.Send(new GetIncidentsQuery(new PagedRequest(pageNumber, pageSize)), cancellationToken);
        return Results.Ok(incidents);
    }
}
