using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.Create;

public sealed class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, Result<IncidentDetailResponse>>
{
    private const string ServiceName = "SecurityCore";

    private readonly IIncidentRepository _incidentRepository;
    private readonly IAuditLogClient _auditLogClient;

    public CreateIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IAuditLogClient auditLogClient)
    {
        _incidentRepository = incidentRepository;
        _auditLogClient = auditLogClient;
    }

    public async Task<Result<IncidentDetailResponse>> Handle(CreateIncidentCommand command, CancellationToken cancellationToken)
    {
        var incident = new Incident(
            Guid.NewGuid(),
            command.Title.Trim(),
            command.Description.Trim(),
            command.Severity,
            string.IsNullOrWhiteSpace(command.AssetName) ? null : command.AssetName.Trim(),
            command.CreatedBy.Trim(),
            DateTimeOffset.UtcNow);

        await _incidentRepository.AddAsync(incident, cancellationToken);

        await _auditLogClient.TryWriteAsync(
            new AuditLogWriteRequest(
                ServiceName,
                "IncidentCreated",
                AuditLogLevel.Information,
                $"Incident '{incident.Title}' was created.",
                "Incident",
                incident.Id.ToString(),
                command.CreatedBy,
                command.CreatedBy,
                null,
                null,
                DateTimeOffset.UtcNow),
            cancellationToken);

        return Result<IncidentDetailResponse>.Success(incident.ToDetailResponse());
    }
}
