using SecurityPlatform.BuildingBlocks.Results;

namespace SecurityAudit.Api.Features.AuditLogs;

public static class AuditErrors
{
    public static Error AuditLogNotFound =>
        new("AuditLog.NotFound", "Audit log was not found.", 404);
}
