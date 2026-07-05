namespace SecurityAudit.Api.Domain.AuditLogs;

public sealed class AuditLog
{
    public Guid Id { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public AuditLogLevel Level { get; init; }
    public string Details { get; init; } = string.Empty;
    public string? ResourceType { get; init; }
    public string? ResourceId { get; init; }
    public string? ActorId { get; init; }
    public string? ActorName { get; init; }
    public string? CorrelationId { get; init; }
    public string? MetadataJson { get; init; }
}
