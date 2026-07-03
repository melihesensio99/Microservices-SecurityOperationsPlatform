using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.GetNotes;

public sealed class GetIncidentNotesQueryHandler : IRequestHandler<GetIncidentNotesQuery, IReadOnlyList<IncidentNoteResponse>?>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentNotesQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<IReadOnlyList<IncidentNoteResponse>?> Handle(
        GetIncidentNotesQuery query,
        CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(query.IncidentId, cancellationToken);
        if (incident is null)
        {
            return null;
        }

        return incident.Notes
            .Select(note => new IncidentNoteResponse(note.Id, note.Author, note.Message, note.CreatedAt))
            .ToList();
    }
}
