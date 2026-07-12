using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SecurityPlatform.BuildingBlocks.Diagnostics;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SecurityPlatform.BuildingBlocks.DependencyInjection;

public static class SecurityPlatformLoggingExtensions
{
    public static IHostBuilder AddSecurityPlatformLogging(
        this IHostBuilder hostBuilder,
        string serviceName)
    {
        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            _ = context;
            _ = services;

            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithProperty("Application", "SecurityPlatform")
                .WriteTo.Console(new RenderedCompactJsonFormatter());
        });
    }

    public static IApplicationBuilder UseSecurityPlatformRequestLogging(
        this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
                diagnosticContext.Set("Host", httpContext.Request.Host.Value);
                diagnosticContext.Set("Scheme", httpContext.Request.Scheme);
                diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString());
            };
        });
    }

    public static IApplicationBuilder UseSecurityPlatformLogContext(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityPlatformLogContextMiddleware>();
    }
}
