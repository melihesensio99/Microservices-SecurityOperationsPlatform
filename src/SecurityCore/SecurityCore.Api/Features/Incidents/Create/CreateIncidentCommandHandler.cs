using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using MediatR;

namespace SecurityCore.Api.Features.Incidents.Create;

public sealed class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, Result<IncidentDetailResponse>>
{
    private readonly IIncidentRepository _incidentRepository;

    public CreateIncidentCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<Result<IncidentDetailResponse>> Handle(CreateIncidentCommand command, CancellationToken cancellationToken)
    {
        var incident = new Incident(
            Guid.NewGuid(),
            command.Title.Trim(),
            command.Description.Trim(),
            command.Severity,
            string.IsNullOrWhiteSpace(command.AssetName) ? null : command.AssetName.Trim(),
            command.CreatedBy.Trim(),
            DateTimeOffset.UtcNow);

        await _incidentRepository.AddAsync(incident, cancellationToken);

        return Result<IncidentDetailResponse>.Success(incident.ToDetailResponse());
    }
}
