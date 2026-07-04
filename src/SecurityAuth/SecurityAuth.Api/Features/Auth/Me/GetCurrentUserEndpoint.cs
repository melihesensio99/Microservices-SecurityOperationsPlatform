using System.Security.Claims;

namespace SecurityAuth.Api.Features.Auth.Me;

public static class GetCurrentUserEndpoint
{
    public static RouteGroupBuilder MapGetCurrentUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization();

        return group;
    }

    private static IResult GetCurrentUserAsync(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return Results.Unauthorized();
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var roles = user.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();

        _ = Guid.TryParse(userId, out var parsedUserId);

        return Results.Ok(new CurrentUserResponse(
            parsedUserId,
            username,
            email,
            roles));
    }
}
