using Microsoft.EntityFrameworkCore;
using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using SecurityCore.Api.Features.Incidents.Create;
using SecurityCore.Api.Infrastructure.Errors;
using SecurityCore.Api.Infrastructure.Persistence;
using SecurityPlatform.BuildingBlocks.DependencyInjection;

namespace SecurityCore.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddDbContext<SecurityCoreDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SecurityDb")));
        services.AddScoped<IIncidentRepository, EfIncidentRepository>();
        services.AddSecurityPlatformMediatR(typeof(CreateIncidentCommand).Assembly);

        return services;
    }
}
