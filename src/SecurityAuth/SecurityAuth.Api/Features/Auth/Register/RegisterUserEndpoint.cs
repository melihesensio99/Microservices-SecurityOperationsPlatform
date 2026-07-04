namespace SecurityAuth.Api.Features.Auth.Register;

public sealed record RegisterUserRequest(
    string Username,
    string Email,
    string Password);

public static class RegisterUserEndpoint
{
    public static RouteGroupBuilder MapRegisterUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/register", RegisterUserAsync);
        return group;
    }

    private static async Task<IResult> RegisterUserAsync(
        RegisterUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Username, request.Email, request.Password);
        var session = await sender.Send(command, cancellationToken);

        return session is null
            ? Results.Conflict("A user with the same username or email already exists.")
            : Results.Created($"/api/auth/me", session);
    }
}
