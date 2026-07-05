using MediatR;
using SecurityAudit.Api.Features.AuditLogs.Abstractions;

namespace SecurityAudit.Api.Features.AuditLogs.GetById;

public sealed class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, Result<AuditLogDetailResponse>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogByIdQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<AuditLogDetailResponse>> Handle(
        GetAuditLogByIdQuery query,
        CancellationToken cancellationToken)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(query.Id, cancellationToken);

        return auditLog is null
            ? Result<AuditLogDetailResponse>.Failure(AuditErrors.AuditLogNotFound)
            : Result<AuditLogDetailResponse>.Success(auditLog.ToDetailResponse());
    }
}
