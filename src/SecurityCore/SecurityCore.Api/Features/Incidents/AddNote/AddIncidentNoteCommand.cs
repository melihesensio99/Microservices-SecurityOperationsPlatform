using SecurityCore.Api.Features.Incidents;
using SecurityPlatform.BuildingBlocks.Cqrs;

namespace SecurityCore.Api.Features.Incidents.AddNote;

public sealed record AddIncidentNoteCommand(
    Guid IncidentId,
    string Author,
    string Message) : ICommand<IncidentDetailResponse>;
