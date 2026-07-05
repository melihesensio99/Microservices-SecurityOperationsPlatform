using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityPlatform.BuildingBlocks.Audit;
using SecurityPlatform.BuildingBlocks.Diagnostics;
using SecurityPlatform.BuildingBlocks.Validation;
using RabbitMQ.Client;

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
        services.Configure<AuditMessagingOptions>(configuration.GetSection("AuditMessaging"));
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AuditMessagingOptions>>().Value;

            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.UserName,
                Password = options.Password,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };
        });
        services.AddSingleton<IAuditLogClient, AuditLogClient>();

        return services;
    }
}
