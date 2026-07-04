using MediatR;
using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;

namespace SecurityCore.Api.Features.Incidents.UpdateStatus;

public sealed class UpdateIncidentStatusCommandHandler : IRequestHandler<UpdateIncidentStatusCommand, IncidentDetailResponse?>
{
    private readonly IIncidentRepository _incidentRepository;

    public UpdateIncidentStatusCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<IncidentDetailResponse?> Handle(UpdateIncidentStatusCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetTrackedByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
        {
            return null;
        }

        incident.ChangeStatus(request.Status, DateTimeOffset.UtcNow);
        await _incidentRepository.SaveChangesAsync(cancellationToken);

        return incident.ToDetailResponse();
    }
}
