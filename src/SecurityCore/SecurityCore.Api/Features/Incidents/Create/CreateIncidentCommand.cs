using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Domain.Incidents;
using SecurityPlatform.BuildingBlocks.Cqrs;

namespace SecurityCore.Api.Features.Incidents.Create;

public sealed record CreateIncidentCommand(
    string Title,
    string Description,
    IncidentSeverity Severity,
    string? AssetName,
    string CreatedBy) : ICommand<IncidentDetailResponse>;
