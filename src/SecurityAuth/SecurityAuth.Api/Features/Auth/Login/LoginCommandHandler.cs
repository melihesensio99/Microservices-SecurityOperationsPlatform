using Microsoft.AspNetCore.Identity;

namespace SecurityAuth.Api.Features.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthSessionResponse>>
{
    private const string ServiceName = "SecurityAuth";

    private readonly IUserStore _userStore;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IAuditLogClient _auditLogClient;

    public LoginCommandHandler(
        IUserStore userStore,
        PasswordHasher<ApplicationUser> passwordHasher,
        ITokenService tokenService,
        IAuditLogClient auditLogClient)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _auditLogClient = auditLogClient;
    }

    public async Task<Result<AuthSessionResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var user = await _userStore.GetByUsernameAsync(username, cancellationToken);
        if (user is null || user.PasswordHash is null)
        {
            await _auditLogClient.TryWriteAsync(
                new AuditLogWriteRequest(
                    ServiceName,
                    "LoginFailed",
                    AuditLogLevel.Warning,
                    "Login attempt failed because the user was not found.",
                    "User",
                    username,
                    null,
                    username,
                    null,
                    null,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            return Result<AuthSessionResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            await _auditLogClient.TryWriteAsync(
                new AuditLogWriteRequest(
                    ServiceName,
                    "LoginFailed",
                    AuditLogLevel.Warning,
                    "Login attempt failed because the password was invalid.",
                    "User",
                    user.Id.ToString(),
                    user.Id.ToString(),
                    username,
                    null,
                    null,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            return Result<AuthSessionResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        await _auditLogClient.TryWriteAsync(
            new AuditLogWriteRequest(
                ServiceName,
                "UserLoggedIn",
                AuditLogLevel.Security,
                "User logged in successfully.",
                "User",
                user.Id.ToString(),
                user.Id.ToString(),
                username,
                null,
                null,
                DateTimeOffset.UtcNow),
            cancellationToken);

        return Result<AuthSessionResponse>.Success(_tokenService.CreateSession(user));
    }
}
