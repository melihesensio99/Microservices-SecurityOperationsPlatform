namespace SecurityAudit.Api.Features.AuditLogs.Create;

public sealed record CreateAuditLogCommand(
    string ServiceName,
    string Action,
    AuditLogLevel Level,
    string Details,
    string? ResourceType,
    string? ResourceId,
    string? ActorId,
    string? ActorName,
    string? CorrelationId,
    string? MetadataJson,
    DateTimeOffset? OccurredAt) : ICommand<Result<AuditLogDetailResponse>>;
