using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents;
using SecurityPlatform.BuildingBlocks.Results;
using SecurityPlatform.BuildingBlocks.Cqrs;

namespace SecurityCore.Api.Features.Incidents.UpdateStatus;

public sealed record UpdateIncidentStatusCommand(
    Guid IncidentId,
    IncidentStatus Status) : ICommand<Result<IncidentDetailResponse>>;
