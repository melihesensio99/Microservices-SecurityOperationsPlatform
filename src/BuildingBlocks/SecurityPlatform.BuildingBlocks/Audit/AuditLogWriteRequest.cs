namespace SecurityPlatform.BuildingBlocks.Audit;

public sealed record AuditLogWriteRequest(
    string ServiceName,
    string Action,
    AuditLogLevel Level,
    string Details,
    string? ResourceType = null,
    string? ResourceId = null,
    string? ActorId = null,
    string? ActorName = null,
    string? CorrelationId = null,
    string? MetadataJson = null,
    DateTimeOffset? OccurredAt = null);
