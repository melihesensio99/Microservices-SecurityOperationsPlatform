using Microsoft.AspNetCore.Http;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public sealed class HttpCorrelationIdProvider(IHttpContextAccessor httpContextAccessor) : ICorrelationIdProvider
{
    public string? GetCorrelationId()
    {
        return httpContextAccessor.HttpContext?.TraceIdentifier;
    }
}
