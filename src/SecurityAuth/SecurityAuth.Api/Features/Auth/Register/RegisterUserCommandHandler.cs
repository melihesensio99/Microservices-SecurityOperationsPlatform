using Microsoft.AspNetCore.Identity;

namespace SecurityAuth.Api.Features.Auth.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthSessionResponse>>
{
    private readonly IUserStore _userStore;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(
        IUserStore userStore,
        PasswordHasher<ApplicationUser> passwordHasher,
        ITokenService tokenService)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthSessionResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();

        var existingByUsername = await _userStore.GetByUsernameAsync(username, cancellationToken);
        var existingByEmail = await _userStore.GetByEmailAsync(email, cancellationToken);

        if (existingByUsername is not null || existingByEmail is not null)
        {
            return Result<AuthSessionResponse>.Failure(AuthErrors.UserAlreadyExists);
        }

        var user = new ApplicationUser(Guid.NewGuid(), username, email);
        user.AddRole("Analyst");
        user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));

        var added = await _userStore.TryAddAsync(user, cancellationToken);
        if (!added)
        {
            return Result<AuthSessionResponse>.Failure(AuthErrors.UserAlreadyExists);
        }

        return Result<AuthSessionResponse>.Success(_tokenService.CreateSession(user));
    }
}
