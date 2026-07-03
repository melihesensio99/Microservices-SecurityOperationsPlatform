namespace SecurityCore.Api.Domain.Incidents;

public sealed class Incident
{
    private readonly List<IncidentNote> _notes = [];

    public Incident(
        Guid id,
        string title,
        string description,
        IncidentSeverity severity,
        string? assetName,
        string createdBy,
        DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Severity = severity;
        AssetName = assetName;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        Status = IncidentStatus.New;
    }

    public Guid Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public string? AssetName { get; private set; }
    public IncidentStatus Status { get; private set; }
    public string CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public IReadOnlyCollection<IncidentNote> Notes => _notes;

    public void AddNote(string author, string message, DateTimeOffset createdAt)
    {
        _notes.Add(new IncidentNote(Guid.NewGuid(), Id, author, message, createdAt));
        UpdatedAt = createdAt;
    }

    public void ChangeStatus(IncidentStatus status, DateTimeOffset updatedAt)
    {
        Status = status;
        UpdatedAt = updatedAt;
    }
}
