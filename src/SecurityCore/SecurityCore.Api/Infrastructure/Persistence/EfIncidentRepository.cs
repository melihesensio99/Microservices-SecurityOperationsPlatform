using Microsoft.EntityFrameworkCore;
using SecurityCore.Api.Domain.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;

namespace SecurityCore.Api.Infrastructure.Persistence;

public sealed class EfIncidentRepository : IIncidentRepository
{
    private readonly SecurityCoreDbContext _dbContext;

    public EfIncidentRepository(SecurityCoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Incident incident, CancellationToken cancellationToken)
    {
        await _dbContext.Incidents.AddAsync(incident, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Incidents
            .AsNoTracking()
            .Include(incident => incident.Notes)
            .FirstOrDefaultAsync(incident => incident.Id == id, cancellationToken);
    }

    public async Task<Incident?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Incidents
            .Include(incident => incident.Notes)
            .FirstOrDefaultAsync(incident => incident.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Incident>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Incidents
            .AsNoTracking()
            .OrderByDescending(incident => incident.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Incident>> GetPageAsync(int skip, int take, CancellationToken cancellationToken)
    {
        return await _dbContext.Incidents
            .AsNoTracking()
            .OrderByDescending(incident => incident.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Incidents.CountAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
