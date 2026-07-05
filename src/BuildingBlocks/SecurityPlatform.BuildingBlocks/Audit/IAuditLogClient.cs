namespace SecurityPlatform.BuildingBlocks.Audit;

public interface IAuditLogClient
{
    Task<bool> TryWriteAsync(AuditLogWriteRequest request, CancellationToken cancellationToken);
}
