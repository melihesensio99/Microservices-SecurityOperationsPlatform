using Microsoft.AspNetCore.Identity;

namespace SecurityAuth.Api.Features.Auth.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthSessionResponse>>
{
    private const string ServiceName = "SecurityAuth";

    private readonly IUserStore _userStore;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IAuditLogClient _auditLogClient;

    public RegisterUserCommandHandler(
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

    public async Task<Result<AuthSessionResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();

        var existingByUsername = await _userStore.GetByUsernameAsync(username, cancellationToken);
        var existingByEmail = await _userStore.GetByEmailAsync(email, cancellationToken);

        if (existingByUsername is not null || existingByEmail is not null)
        {
            await _auditLogClient.TryWriteAsync(
                new AuditLogWriteRequest(
                    ServiceName,
                    "RegisterRejected",
                    AuditLogLevel.Warning,
                    "Registration was rejected because the username or email already exists.",
                    "User",
                    username,
                    null,
                    username,
                    null,
                    null,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            return Result<AuthSessionResponse>.Failure(AuthErrors.UserAlreadyExists);
        }

        var user = new ApplicationUser(Guid.NewGuid(), username, email);
        user.AddRole("Analyst");
        user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));

        var added = await _userStore.TryAddAsync(user, cancellationToken);
        if (!added)
        {
            await _auditLogClient.TryWriteAsync(
                new AuditLogWriteRequest(
                    ServiceName,
                    "RegisterRejected",
                    AuditLogLevel.Warning,
                    "Registration was rejected while saving the new user.",
                    "User",
                    username,
                    null,
                    username,
                    null,
                    null,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            return Result<AuthSessionResponse>.Failure(AuthErrors.UserAlreadyExists);
        }

        await _auditLogClient.TryWriteAsync(
            new AuditLogWriteRequest(
                ServiceName,
                "UserRegistered",
                AuditLogLevel.Security,
                "New user account was created successfully.",
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
