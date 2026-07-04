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
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/auth/me", result.Value)
            : Results.Json(
                new { error = result.Error.Message, code = result.Error.Code },
                statusCode: result.Error.StatusCode);
    }
}
