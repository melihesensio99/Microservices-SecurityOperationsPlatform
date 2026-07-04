using Microsoft.EntityFrameworkCore;
using SecurityCore.Api.Features.Incidents;
using SecurityCore.Api.Features.Incidents.Abstractions;
using SecurityCore.Api.Features.Incidents.Create;
using SecurityCore.Api.Infrastructure.Errors;
using SecurityCore.Api.Infrastructure.Persistence;

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
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<CreateIncidentCommand>();
            config.AddOpenBehavior(typeof(SecurityPlatform.BuildingBlocks.Diagnostics.LoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(SecurityPlatform.BuildingBlocks.Validation.ValidationPipelineBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining<CreateIncidentRequestValidator>();

        return services;
    }
}
