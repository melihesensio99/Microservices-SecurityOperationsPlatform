using SecurityAudit.Api.Domain.AuditLogs;

namespace SecurityAudit.Api.Features.AuditLogs;

public static class AuditMappings
{
    public static AuditLogSummaryResponse ToSummaryResponse(this AuditLog auditLog)
    {
        return new AuditLogSummaryResponse(
            auditLog.Id,
            auditLog.OccurredAt,
            auditLog.ServiceName,
            auditLog.Action,
            auditLog.Level,
            auditLog.ResourceType,
            auditLog.ResourceId,
            auditLog.ActorName);
    }

    public static AuditLogDetailResponse ToDetailResponse(this AuditLog auditLog)
    {
        return new AuditLogDetailResponse(
            auditLog.Id,
            auditLog.OccurredAt,
            auditLog.ServiceName,
            auditLog.Action,
            auditLog.Level,
            auditLog.Details,
            auditLog.ResourceType,
            auditLog.ResourceId,
            auditLog.ActorId,
            auditLog.ActorName,
            auditLog.CorrelationId,
            auditLog.MetadataJson);
    }
}
