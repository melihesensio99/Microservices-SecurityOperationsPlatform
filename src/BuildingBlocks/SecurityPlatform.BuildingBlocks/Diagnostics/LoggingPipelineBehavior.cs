using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public sealed class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startedAt = Stopwatch.StartNew();
        var requestPayload = JsonSerializer.Serialize(request, request!.GetType());

        _logger.LogInformation("Handling {RequestName}. Payload: {RequestPayload}", requestName, requestPayload);

        try
        {
            var response = await next();
            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds} ms",
                requestName,
                startedAt.Elapsed.TotalMilliseconds);
            return response;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed {RequestName} after {ElapsedMilliseconds} ms",
                requestName,
                startedAt.Elapsed.TotalMilliseconds);
            throw;
        }
    }
}
