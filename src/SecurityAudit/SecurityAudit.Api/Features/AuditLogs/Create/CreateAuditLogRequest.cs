using SecurityPlatform.BuildingBlocks.Audit;

namespace SecurityAudit.Api.Features.AuditLogs.Create;

public sealed record CreateAuditLogRequest(
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
    DateTimeOffset? OccurredAt);
