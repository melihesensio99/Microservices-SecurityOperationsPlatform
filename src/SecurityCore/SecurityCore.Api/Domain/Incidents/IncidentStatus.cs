namespace SecurityCore.Api.Domain.Incidents;

public enum IncidentStatus
{
    New = 0,
    Investigating = 1,
    Mitigated = 2,
    Resolved = 3,
    Closed = 4
}
