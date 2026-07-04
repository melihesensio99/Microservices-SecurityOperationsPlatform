using SecurityAuth.Api.Infrastructure.UserContext;

namespace SecurityAuth.Api.Features.Auth.Me;

public static class GetCurrentUserEndpoint
{
    public static RouteGroupBuilder MapGetCurrentUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization();

        return group;
    }

    private static IResult GetCurrentUserAsync(ICurrentUserService currentUserService)
    {
        var currentUser = currentUserService.GetCurrentUser();

        if (currentUser is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(currentUser);
    }
}
