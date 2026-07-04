namespace SecurityAuth.Api.Features.Auth;

public sealed record AuthSessionResponse(
    Guid UserId,
    string Username,
    string Email,
    string Token,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<string> Roles);

public sealed record CurrentUserResponse(
    Guid UserId,
    string Username,
    string Email,
    IReadOnlyList<string> Roles);
