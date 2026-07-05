using FluentValidation;
using SecurityPlatform.BuildingBlocks.Audit;

namespace SecurityAudit.Api.Features.AuditLogs.Create;

public sealed class CreateAuditLogRequestValidator : AbstractValidator<CreateAuditLogRequest>
{
    public CreateAuditLogRequestValidator()
    {
        RuleFor(request => request.ServiceName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(request => request.Action)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Details)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(request => request.ResourceType)
            .MaximumLength(128);

        RuleFor(request => request.ResourceId)
            .MaximumLength(128);

        RuleFor(request => request.ActorId)
            .MaximumLength(128);

        RuleFor(request => request.ActorName)
            .MaximumLength(256);

        RuleFor(request => request.CorrelationId)
            .MaximumLength(128);

        RuleFor(request => request.MetadataJson)
            .MaximumLength(4000);
    }
}
