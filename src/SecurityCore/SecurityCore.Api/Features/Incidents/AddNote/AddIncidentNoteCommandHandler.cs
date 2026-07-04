using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.AddNote;

public sealed class AddIncidentNoteCommandHandler : IRequestHandler<AddIncidentNoteCommand, Result<IncidentDetailResponse>>
{
    private readonly IIncidentRepository _incidentRepository;

    public AddIncidentNoteCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<Result<IncidentDetailResponse>> Handle(AddIncidentNoteCommand command, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetTrackedByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null)
        {
            return Result<IncidentDetailResponse>.Failure(IncidentErrors.IncidentNotFound);
        }

        incident.AddNote(
            command.Author.Trim(),
            command.Message.Trim(),
            DateTimeOffset.UtcNow);

        await _incidentRepository.SaveChangesAsync(cancellationToken);

        return Result<IncidentDetailResponse>.Success(incident.ToDetailResponse());
    }
}
