using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public sealed class DbContextHealthCheck<TDbContext> : IHealthCheck
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public DbContextHealthCheck(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy($"{typeof(TDbContext).Name} is reachable")
                : HealthCheckResult.Unhealthy($"{typeof(TDbContext).Name} is not reachable");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                $"{typeof(TDbContext).Name} health check failed",
                exception);
        }
    }
}
