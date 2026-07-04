namespace SecurityAuth.Api.Features.Auth.Register;

public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string Password) : ICommand<AuthSessionResponse?>;
