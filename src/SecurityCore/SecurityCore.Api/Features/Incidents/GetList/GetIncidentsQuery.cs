using SecurityCore.Api.Features.Incidents;
using SecurityPlatform.BuildingBlocks.Cqrs;
using SecurityPlatform.BuildingBlocks.Pagination;

namespace SecurityCore.Api.Features.Incidents.GetList;

public sealed record GetIncidentsQuery(PagedRequest Pagination) : IQuery<PagedResult<IncidentSummaryResponse>>;
