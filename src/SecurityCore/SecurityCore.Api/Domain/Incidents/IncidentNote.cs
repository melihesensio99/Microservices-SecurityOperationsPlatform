namespace SecurityCore.Api.Domain.Incidents;

public sealed class IncidentNote
{
    public IncidentNote(Guid id, Guid incidentId, string author, string message, DateTimeOffset createdAt)
    {
        Id = id;
        IncidentId = incidentId;
        Author = author;
        Message = message;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public Guid IncidentId { get; }
    public string Author { get; }
    public string Message { get; }
    public DateTimeOffset CreatedAt { get; }
}
