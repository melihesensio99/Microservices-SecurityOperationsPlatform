using System.Security.Claims;
using SecurityAuth.Api.Features.Auth;

namespace SecurityAuth.Api.Infrastructure.UserContext;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public CurrentUserResponse? GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var roles = user.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();

        _ = Guid.TryParse(userId, out var parsedUserId);

        return new CurrentUserResponse(parsedUserId, username, email, roles);
    }
}
