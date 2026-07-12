using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityPlatform.BuildingBlocks.Audit;
using SecurityPlatform.BuildingBlocks.Diagnostics;
using SecurityPlatform.BuildingBlocks.Validation;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace SecurityPlatform.BuildingBlocks.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityPlatformMediatR(
        this IServiceCollection services,
        Assembly applicationAssembly)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICorrelationIdProvider, HttpCorrelationIdProvider>();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.AddOpenBehavior(typeof(TracingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IServiceCollection AddSecurityPlatformTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.Configure<TracingOptions>(configuration.GetSection("OpenTelemetry"));

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddSource(SecurityPlatformActivitySource.SourceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext =>
                            !httpContext.Request.Path.StartsWithSegments("/health");
                    });

                var tracingOptions = configuration.GetSection("OpenTelemetry").Get<TracingOptions>() ?? new TracingOptions();
                if (tracingOptions.Enabled)
                {
                    tracing.AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(tracingOptions.OtlpEndpoint);
                    });
                }
            });

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
