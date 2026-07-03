using FluentValidation;

namespace SecurityCore.Api.Features.Incidents.Create;

public sealed class CreateIncidentRequestValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Severity)
            .IsInEnum();

        RuleFor(x => x.AssetName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.AssetName));

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}
