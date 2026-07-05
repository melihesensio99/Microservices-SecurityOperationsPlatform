using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public static class HealthEndpointExtensions
{
    public static IEndpointRouteBuilder MapSecurityPlatformHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var health = endpoints.MapGroup("/health");

        health.MapGet("/live", () => global::Microsoft.AspNetCore.Http.Results.Ok(new { status = "Alive" }));
        health.MapGet("/ready", async (HttpContext httpContext) =>
        {
            var healthReport = await httpContext.RequestServices
                .GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>()
                .CheckHealthAsync();

            var response = new
            {
                status = healthReport.Status.ToString(),
                entries = healthReport.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description
                })
            };

            return global::Microsoft.AspNetCore.Http.Results.Ok(response);
        });

        return endpoints;
    }
}
