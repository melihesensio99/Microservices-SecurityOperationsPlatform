using MediatR;
using SecurityAudit.Api.Features.AuditLogs.Abstractions;

namespace SecurityAudit.Api.Features.AuditLogs.GetList;

public sealed class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogSummaryResponse>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<AuditLogSummaryResponse>> Handle(
        GetAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        var pagination = query.Pagination.Normalize();
        var totalCount = await _auditLogRepository.CountAsync(query.ServiceName, query.Level, cancellationToken);
        var auditLogs = await _auditLogRepository.GetPageAsync(
            pagination.Skip,
            pagination.PageSize,
            query.ServiceName,
            query.Level,
            cancellationToken);

        return new PagedResult<AuditLogSummaryResponse>(
            auditLogs.Select(auditLog => auditLog.ToSummaryResponse()).ToList(),
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }
}
