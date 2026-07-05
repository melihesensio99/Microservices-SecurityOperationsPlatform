using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.AddNote;

public sealed class AddIncidentNoteCommandHandler : IRequestHandler<AddIncidentNoteCommand, Result<IncidentDetailResponse>>
{
    private const string ServiceName = "SecurityCore";

    private readonly IIncidentRepository _incidentRepository;
    private readonly IAuditLogClient _auditLogClient;

    public AddIncidentNoteCommandHandler(
        IIncidentRepository incidentRepository,
        IAuditLogClient auditLogClient)
    {
        _incidentRepository = incidentRepository;
        _auditLogClient = auditLogClient;
    }

    public async Task<Result<IncidentDetailResponse>> Handle(AddIncidentNoteCommand command, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetTrackedByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null)
        {
            await _auditLogClient.TryWriteAsync(
                new AuditLogWriteRequest(
                    ServiceName,
                    "IncidentNoteRejected",
                    AuditLogLevel.Warning,
                    "Incident note could not be added because the incident was not found.",
                    "Incident",
                    command.IncidentId.ToString(),
                    null,
                    command.Author,
                    null,
                    null,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            return Result<IncidentDetailResponse>.Failure(IncidentErrors.IncidentNotFound);
        }

        incident.AddNote(
            command.Author.Trim(),
            command.Message.Trim(),
            DateTimeOffset.UtcNow);

        await _incidentRepository.SaveChangesAsync(cancellationToken);

        await _auditLogClient.TryWriteAsync(
            new AuditLogWriteRequest(
                ServiceName,
                "IncidentNoteAdded",
                AuditLogLevel.Information,
                "A note was added to the incident.",
                "Incident",
                command.IncidentId.ToString(),
                null,
                command.Author,
                null,
                null,
                DateTimeOffset.UtcNow),
            cancellationToken);

        return Result<IncidentDetailResponse>.Success(incident.ToDetailResponse());
    }
}
