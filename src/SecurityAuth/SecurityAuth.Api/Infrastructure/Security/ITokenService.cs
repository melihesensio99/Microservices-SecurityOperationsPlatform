namespace SecurityAuth.Api.Infrastructure.Security;

public interface ITokenService
{
    AuthSessionResponse CreateSession(ApplicationUser user);
}
