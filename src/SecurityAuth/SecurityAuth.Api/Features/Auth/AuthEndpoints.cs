using SecurityAuth.Api.Features.Auth.Login;
using SecurityAuth.Api.Features.Auth.Me;
using SecurityAuth.Api.Features.Auth.Register;

namespace SecurityAuth.Api.Features.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group
            .MapRegisterUserEndpoint()
            .MapLoginEndpoint()
            .MapGetCurrentUserEndpoint();

        return app;
    }
}
