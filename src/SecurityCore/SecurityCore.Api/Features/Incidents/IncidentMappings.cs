using SecurityCore.Api.Domain.Incidents;

namespace SecurityCore.Api.Features.Incidents;

public static class IncidentMappings
{
    public static IncidentSummaryResponse ToSummaryResponse(this Incident incident)
    {
        return new IncidentSummaryResponse(
            incident.Id,
            incident.Title,
            incident.Severity,
            incident.Status,
            incident.CreatedAt);
    }

    public static IncidentDetailResponse ToDetailResponse(this Incident incident)
    {
        return new IncidentDetailResponse(
            incident.Id,
            incident.Title,
            incident.Description,
            incident.Severity,
            incident.Status,
            incident.AssetName,
            incident.CreatedBy,
            incident.CreatedAt,
            incident.UpdatedAt,
            incident.Notes.Select(note => new IncidentNoteResponse(note.Id, note.Author, note.Message, note.CreatedAt)).ToList());
    }
}
