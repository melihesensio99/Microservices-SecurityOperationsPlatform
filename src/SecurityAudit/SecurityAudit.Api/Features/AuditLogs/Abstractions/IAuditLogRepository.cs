using SecurityAudit.Api.Domain.AuditLogs;

namespace SecurityAudit.Api.Features.AuditLogs.Abstractions;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken);

    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<AuditLog>> GetPageAsync(
        int skip,
        int take,
        string? serviceName,
        AuditLogLevel? level,
        CancellationToken cancellationToken);

    Task<int> CountAsync(string? serviceName, AuditLogLevel? level, CancellationToken cancellationToken);
}
