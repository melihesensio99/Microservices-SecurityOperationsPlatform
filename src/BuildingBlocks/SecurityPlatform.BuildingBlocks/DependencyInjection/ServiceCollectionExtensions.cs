using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityPlatform.BuildingBlocks.Audit;
using SecurityPlatform.BuildingBlocks.Diagnostics;
using SecurityPlatform.BuildingBlocks.Validation;

namespace SecurityPlatform.BuildingBlocks.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityPlatformMediatR(
        this IServiceCollection services,
        Assembly applicationAssembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IServiceCollection AddSecurityPlatformAuditClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuditLoggingOptions>(configuration.GetSection("Audit"));
        services.AddHttpClient<IAuditLogClient, AuditLogClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AuditLoggingOptions>>().Value;
            var baseUrl = string.IsNullOrWhiteSpace(options.BaseUrl)
                ? "http://localhost:5336"
                : options.BaseUrl.TrimEnd('/');

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(3);
        });

        return services;
    }
}
