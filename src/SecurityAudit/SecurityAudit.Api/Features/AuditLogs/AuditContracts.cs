namespace SecurityAudit.Api.Features.AuditLogs;

public sealed record AuditLogSummaryResponse(
    Guid Id,
    DateTimeOffset OccurredAt,
    string ServiceName,
    string Action,
    AuditLogLevel Level,
    string? ResourceType,
    string? ResourceId,
    string? ActorName);

public sealed record AuditLogDetailResponse(
    Guid Id,
    DateTimeOffset OccurredAt,
    string ServiceName,
    string Action,
    AuditLogLevel Level,
    string Details,
    string? ResourceType,
    string? ResourceId,
    string? ActorId,
    string? ActorName,
    string? CorrelationId,
    string? MetadataJson);
