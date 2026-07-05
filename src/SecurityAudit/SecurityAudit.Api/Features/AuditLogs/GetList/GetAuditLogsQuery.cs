namespace SecurityAudit.Api.Features.AuditLogs.GetList;

public sealed record GetAuditLogsQuery(
    PagedRequest Pagination,
    string? ServiceName,
    AuditLogLevel? Level) : IQuery<PagedResult<AuditLogSummaryResponse>>;
