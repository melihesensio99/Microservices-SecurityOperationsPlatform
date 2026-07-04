using SecurityCore.Api.Features.Incidents;
using SecurityPlatform.BuildingBlocks.Results;
using SecurityPlatform.BuildingBlocks.Cqrs;

namespace SecurityCore.Api.Features.Incidents.GetById;

public sealed record GetIncidentByIdQuery(Guid Id) : IQuery<Result<IncidentDetailResponse>>;
