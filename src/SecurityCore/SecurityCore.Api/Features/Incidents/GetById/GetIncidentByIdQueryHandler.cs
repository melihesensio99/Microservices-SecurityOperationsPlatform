using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.GetById;

public sealed class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, Result<IncidentDetailResponse>>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentByIdQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<Result<IncidentDetailResponse>> Handle(GetIncidentByIdQuery query, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(query.Id, cancellationToken);
        return incident is null
            ? Result<IncidentDetailResponse>.Failure(IncidentErrors.IncidentNotFound)
            : Result<IncidentDetailResponse>.Success(incident.ToDetailResponse());
    }
}
