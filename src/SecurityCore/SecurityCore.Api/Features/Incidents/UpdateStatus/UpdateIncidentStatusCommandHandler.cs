using MediatR;
using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;

namespace SecurityCore.Api.Features.Incidents.UpdateStatus;

public sealed class UpdateIncidentStatusCommandHandler : IRequestHandler<UpdateIncidentStatusCommand, Result<IncidentDetailResponse>>
{
    private const string ServiceName = "SecurityCore";

    private readonly IIncidentRepository _incidentRepository;
    private readonly IAuditLogClient _auditLogClient;

    public UpdateIncidentStatusCommandHandler(
        IIncidentRepository incidentRepository,
        IAuditLogClient auditLogClient)
    {
        _incidentRepository = incidentRepository;
        _auditLogClient = auditLogClient;
    }

    public async Task<Result<IncidentDetailResponse>> Handle(UpdateIncidentStatusCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetTrackedByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
        {
            await _auditLogClient.TryWriteAsync(
                new AuditLogWriteRequest(
                    ServiceName,
                    "IncidentStatusUpdateRejected",
                    AuditLogLevel.Warning,
                    "Incident status update failed because the incident was not found.",
                    "Incident",
                    request.IncidentId.ToString(),
                    null,
                    null,
                    null,
                    null,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            return Result<IncidentDetailResponse>.Failure(IncidentErrors.IncidentNotFound);
        }

        incident.ChangeStatus(request.Status, DateTimeOffset.UtcNow);
        await _incidentRepository.SaveChangesAsync(cancellationToken);

        await _auditLogClient.TryWriteAsync(
            new AuditLogWriteRequest(
                ServiceName,
                "IncidentStatusUpdated",
                AuditLogLevel.Information,
                $"Incident status was updated to {request.Status}.",
                "Incident",
                request.IncidentId.ToString(),
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow),
            cancellationToken);

        return Result<IncidentDetailResponse>.Success(incident.ToDetailResponse());
    }
}
