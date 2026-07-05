namespace SecurityAudit.Api.Features.AuditLogs.GetById;

public sealed record GetAuditLogByIdQuery(Guid Id) : IQuery<Result<AuditLogDetailResponse>>;
