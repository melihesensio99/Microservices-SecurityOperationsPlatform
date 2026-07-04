using SecurityPlatform.BuildingBlocks.Results;

namespace SecurityAuth.Api.Features.Auth;

public static class AuthErrors
{
    public static Error InvalidCredentials =>
        new("Auth.InvalidCredentials", "Username or password is incorrect.", StatusCodes.Status401Unauthorized);

    public static Error UserAlreadyExists =>
        new("Auth.UserAlreadyExists", "A user with the same username or email already exists.", StatusCodes.Status409Conflict);
}
