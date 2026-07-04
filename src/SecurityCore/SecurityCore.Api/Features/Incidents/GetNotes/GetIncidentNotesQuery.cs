using SecurityCore.Api.Features.Incidents;
using SecurityPlatform.BuildingBlocks.Results;
using SecurityPlatform.BuildingBlocks.Cqrs;

namespace SecurityCore.Api.Features.Incidents.GetNotes;

public sealed record GetIncidentNotesQuery(Guid IncidentId) : IQuery<Result<IReadOnlyList<IncidentNoteResponse>>>;
