namespace SecurityAuth.Api.Features.Auth.Login;

public sealed record LoginCommand(
    string Username,
    string Password) : ICommand<Result<AuthSessionResponse>>;
