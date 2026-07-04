using SecurityAuth.Api.Features.Auth;

namespace SecurityAuth.Api.Infrastructure.UserContext;

public interface ICurrentUserService
{
    CurrentUserResponse? GetCurrentUser();
}
