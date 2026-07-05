using Microsoft.EntityFrameworkCore;
using SecurityAudit.Api.Features.AuditLogs.Abstractions;
using SecurityAudit.Api.Features.AuditLogs.Create;
using SecurityAudit.Api.Infrastructure.Messaging;
using SecurityAudit.Api.Infrastructure.Errors;
using SecurityPlatform.BuildingBlocks.DependencyInjection;
using SecurityPlatform.BuildingBlocks.Diagnostics;

namespace SecurityAudit.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityAuditServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddSecurityPlatformAuditClient(configuration);
        services.AddDbContext<AuditDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AuditDb")));
        services.AddHealthChecks()
            .AddCheck<DbContextHealthCheck<AuditDbContext>>("audit-db");
        services.AddScoped<IAuditLogRepository, EfAuditLogRepository>();
        services.AddHostedService<AuditLogQueueConsumer>();
        services.AddSecurityPlatformMediatR(typeof(CreateAuditLogCommand).Assembly);

        return services;
    }
}
