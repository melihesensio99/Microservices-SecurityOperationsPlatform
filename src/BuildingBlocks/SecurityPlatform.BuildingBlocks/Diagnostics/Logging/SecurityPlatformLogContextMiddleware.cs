using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public sealed class SecurityPlatformLogContextMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityPlatformLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var correlationScope = LogContext.PushProperty("CorrelationId", context.TraceIdentifier);
        using var traceScope = LogContext.PushProperty("TraceId", Activity.Current?.TraceId.ToString());
        using var spanScope = LogContext.PushProperty("SpanId", Activity.Current?.SpanId.ToString());
        using var pathScope = LogContext.PushProperty("RequestPath", context.Request.Path.Value);

        await _next(context);
    }
}
