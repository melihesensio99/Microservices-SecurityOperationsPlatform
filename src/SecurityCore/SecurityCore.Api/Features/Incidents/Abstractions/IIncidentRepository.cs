using SecurityCore.Api.Domain.Incidents;

namespace SecurityCore.Api.Features.Incidents.Abstractions;

public interface IIncidentRepository
{
    Task AddAsync(Incident incident, CancellationToken cancellationToken);
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Incident?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Incident>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Incident>> GetPageAsync(int skip, int take, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
