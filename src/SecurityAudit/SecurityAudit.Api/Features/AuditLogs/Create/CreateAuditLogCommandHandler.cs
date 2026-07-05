using SecurityAudit.Api.Features.AuditLogs.Abstractions;
using SecurityPlatform.BuildingBlocks.Results;
using MediatR;

namespace SecurityAudit.Api.Features.AuditLogs.Create;

public sealed class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, Result<AuditLogDetailResponse>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public CreateAuditLogCommandHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<AuditLogDetailResponse>> Handle(
        CreateAuditLogCommand command,
        CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            OccurredAt = command.OccurredAt ?? DateTimeOffset.UtcNow,
            ServiceName = command.ServiceName.Trim(),
            Action = command.Action.Trim(),
            Level = command.Level,
            Details = command.Details.Trim(),
            ResourceType = command.ResourceType?.Trim(),
            ResourceId = command.ResourceId?.Trim(),
            ActorId = command.ActorId?.Trim(),
            ActorName = command.ActorName?.Trim(),
            CorrelationId = command.CorrelationId?.Trim(),
            MetadataJson = command.MetadataJson?.Trim()
        };

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);

        return Result<AuditLogDetailResponse>.Success(auditLog.ToDetailResponse());
    }
}
