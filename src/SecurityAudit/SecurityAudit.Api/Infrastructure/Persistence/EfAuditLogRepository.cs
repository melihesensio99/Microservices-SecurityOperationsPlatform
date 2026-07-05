using Microsoft.EntityFrameworkCore;
using SecurityAudit.Api.Domain.AuditLogs;
using SecurityAudit.Api.Features.AuditLogs.Abstractions;

namespace SecurityAudit.Api.Infrastructure.Persistence;

public sealed class EfAuditLogRepository : IAuditLogRepository
{
    private readonly AuditDbContext _dbContext;

    public EfAuditLogRepository(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        await _dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(auditLog => auditLog.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetPageAsync(
        int skip,
        int take,
        string? serviceName,
        AuditLogLevel? level,
        CancellationToken cancellationToken)
    {
        var query = ApplyFilters(_dbContext.AuditLogs.AsNoTracking(), serviceName, level)
            .OrderByDescending(auditLog => auditLog.OccurredAt);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(string? serviceName, AuditLogLevel? level, CancellationToken cancellationToken)
    {
        return ApplyFilters(_dbContext.AuditLogs.AsNoTracking(), serviceName, level)
            .CountAsync(cancellationToken);
    }

    private static IQueryable<AuditLog> ApplyFilters(
        IQueryable<AuditLog> query,
        string? serviceName,
        AuditLogLevel? level)
    {
        if (!string.IsNullOrWhiteSpace(serviceName))
        {
            query = query.Where(auditLog => auditLog.ServiceName == serviceName);
        }

        if (level is not null)
        {
            query = query.Where(auditLog => auditLog.Level == level);
        }

        return query;
    }
}
