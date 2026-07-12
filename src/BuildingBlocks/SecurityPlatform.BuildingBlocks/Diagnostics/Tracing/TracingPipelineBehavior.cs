using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public sealed class TracingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TracingPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public TracingPipelineBehavior(
        ILogger<TracingPipelineBehavior<TRequest, TResponse>> logger,
        ICorrelationIdProvider correlationIdProvider)
    {
        _logger = logger;
        _correlationIdProvider = correlationIdProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = _correlationIdProvider.GetCorrelationId();

        using var activity = SecurityPlatformActivitySource.ActivitySource.StartActivity(
            requestName,
            ActivityKind.Internal);
        using var logScope = SecurityPlatformLogContext.Push(correlationId);

        if (activity is not null)
        {
            activity.SetTag("request.type", typeof(TRequest).FullName);
            activity.SetTag("correlation.id", correlationId);
        }

        _logger.LogDebug(
            "Tracing {RequestName}.",
            requestName);

        try
        {
            var response = await next();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
    }
}
