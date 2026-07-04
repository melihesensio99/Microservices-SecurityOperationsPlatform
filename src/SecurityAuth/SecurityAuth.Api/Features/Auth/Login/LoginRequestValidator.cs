namespace SecurityAuth.Api.Features.Auth.Login;

public sealed class LoginRequestValidator : AbstractValidator<LoginCommand>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}
