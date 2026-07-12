using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SecurityPlatform.BuildingBlocks.DependencyInjection;

public static class SecurityPlatformMetricsExtensions
{
    public static IServiceCollection AddSecurityPlatformMetrics(
        this IServiceCollection services)
    {
        services.AddSingleton<SecurityPlatform.BuildingBlocks.Diagnostics.Metrics.SecurityPlatformMetricStore>();
        return services;
    }

    public static IEndpointRouteBuilder MapSecurityPlatformMetricsEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/metrics", (SecurityPlatform.BuildingBlocks.Diagnostics.Metrics.SecurityPlatformMetricStore store) =>
            global::Microsoft.AspNetCore.Http.Results.Text(
                store.ExportPrometheus(),
                "text/plain; version=0.0.4; charset=utf-8"));

        return endpoints;
    }
}
