using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.GetById;

public sealed class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, IncidentDetailResponse?>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentByIdQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<IncidentDetailResponse?> Handle(GetIncidentByIdQuery query, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(query.Id, cancellationToken);
        return incident?.ToDetailResponse();
    }
}
