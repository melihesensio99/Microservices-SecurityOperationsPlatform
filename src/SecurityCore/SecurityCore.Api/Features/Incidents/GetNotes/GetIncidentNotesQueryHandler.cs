using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.GetNotes;

public sealed class GetIncidentNotesQueryHandler : IRequestHandler<GetIncidentNotesQuery, Result<IReadOnlyList<IncidentNoteResponse>>>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentNotesQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<Result<IReadOnlyList<IncidentNoteResponse>>> Handle(
        GetIncidentNotesQuery query,
        CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(query.IncidentId, cancellationToken);
        if (incident is null)
        {
            return Result<IReadOnlyList<IncidentNoteResponse>>.Failure(IncidentErrors.IncidentNotFound);
        }

        return Result<IReadOnlyList<IncidentNoteResponse>>.Success(
            incident.Notes
            .Select(note => new IncidentNoteResponse(note.Id, note.Author, note.Message, note.CreatedAt))
            .ToList());
    }
}
