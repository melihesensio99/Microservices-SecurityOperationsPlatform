namespace SecurityAuth.Api.Features.Auth.Login;

public sealed record LoginRequest(
    string Username,
    string Password);

public static class LoginEndpoint
{
    public static RouteGroupBuilder MapLoginEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/login", LoginUserAsync);
        return group;
    }

    private static async Task<IResult> LoginUserAsync(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Username, request.Password);
        var result = await sender.Send(command, cancellationToken);

        return result.ToOkHttpResult();
    }
}
