using Microsoft.AspNetCore.Identity;

namespace SecurityAuth.Api.Features.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthSessionResponse>>
{
    private readonly IUserStore _userStore;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserStore userStore,
        PasswordHasher<ApplicationUser> passwordHasher,
        ITokenService tokenService)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthSessionResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var user = await _userStore.GetByUsernameAsync(username, cancellationToken);
        if (user is null || user.PasswordHash is null)
        {
            return Result<AuthSessionResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            return Result<AuthSessionResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        return Result<AuthSessionResponse>.Success(_tokenService.CreateSession(user));
    }
}
