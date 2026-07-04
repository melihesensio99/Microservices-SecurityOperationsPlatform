namespace SecurityAuth.Api.Features.Auth.Register;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}
