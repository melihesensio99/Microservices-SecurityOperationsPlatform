using FluentValidation;

namespace SecurityCore.Api.Features.Incidents.AddNote;

public sealed class AddIncidentNoteRequestValidator : AbstractValidator<AddIncidentNoteCommand>
{
    public AddIncidentNoteRequestValidator()
    {
        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
