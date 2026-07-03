using SecurityCore.Api.Features.Incidents;
using SecurityPlatform.BuildingBlocks.Cqrs;

namespace SecurityCore.Api.Features.Incidents.GetNotes;

public sealed record GetIncidentNotesQuery(Guid IncidentId) : IQuery<IReadOnlyList<IncidentNoteResponse>>;
