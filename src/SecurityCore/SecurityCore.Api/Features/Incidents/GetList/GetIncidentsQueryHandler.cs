using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using SecurityPlatform.BuildingBlocks.Pagination;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.GetList;

public sealed class GetIncidentsQueryHandler : IRequestHandler<GetIncidentsQuery, PagedResult<IncidentSummaryResponse>>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentsQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<PagedResult<IncidentSummaryResponse>> Handle(GetIncidentsQuery query, CancellationToken cancellationToken)
    {
        var pagination = query.Pagination.Normalize();
        var totalCount = await _incidentRepository.CountAsync(cancellationToken);
        var incidents = await _incidentRepository.GetPageAsync(pagination.Skip, pagination.PageSize, cancellationToken);

        return new PagedResult<IncidentSummaryResponse>(
            incidents.Select(incident => incident.ToSummaryResponse()).ToList(),
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }
}
