using SecurityCore.Api.Domain.Incidents;

namespace SecurityCore.Api.Features.Incidents;

public sealed record IncidentNoteResponse(Guid Id, string Author, string Message, DateTimeOffset CreatedAt);

public sealed record IncidentSummaryResponse(
    Guid Id,
    string Title,
    IncidentSeverity Severity,
    IncidentStatus Status,
    DateTimeOffset CreatedAt);

public sealed record IncidentDetailResponse(
    Guid Id,
    string Title,
    string Description,
    IncidentSeverity Severity,
    IncidentStatus Status,
    string? AssetName,
    string CreatedBy,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<IncidentNoteResponse> Notes);
