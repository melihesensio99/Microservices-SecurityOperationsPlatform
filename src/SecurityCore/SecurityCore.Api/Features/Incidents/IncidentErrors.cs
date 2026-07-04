using SecurityPlatform.BuildingBlocks.Results;

namespace SecurityCore.Api.Features.Incidents;

public static class IncidentErrors
{
    public static Error IncidentNotFound =>
        new("Incident.NotFound", "The incident was not found.", StatusCodes.Status404NotFound);
}
