using System.Text.Json;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SecurityPlatform.BuildingBlocks.Diagnostics;

namespace SecurityPlatform.BuildingBlocks.Audit;

public sealed class AuditLogClient : IAuditLogClient
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly AuditMessagingOptions _options;
    private readonly ILogger<AuditLogClient> _logger;
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public AuditLogClient(
        ConnectionFactory connectionFactory,
        IOptions<AuditMessagingOptions> options,
        ILogger<AuditLogClient> logger,
        ICorrelationIdProvider correlationIdProvider)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
        _logger = logger;
        _correlationIdProvider = correlationIdProvider;
    }

    public Task<bool> TryWriteAsync(AuditLogWriteRequest request, CancellationToken cancellationToken)
    {
        var enrichedRequest = string.IsNullOrWhiteSpace(request.CorrelationId)
            ? request with { CorrelationId = _correlationIdProvider.GetCorrelationId() }
            : request;

        using var activity = SecurityPlatformActivitySource.ActivitySource.StartActivity(
            "Publish audit log",
            ActivityKind.Producer);

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = JsonSerializer.SerializeToUtf8Bytes(enrichedRequest, AuditSerialization.JsonOptions);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Type = nameof(AuditLogWriteRequest);
            properties.CorrelationId = enrichedRequest.CorrelationId;

            if (activity is not null)
            {
                activity.SetTag("messaging.system", "rabbitmq");
                activity.SetTag("messaging.destination", _options.QueueName);
                activity.SetTag("messaging.operation", "publish");
                RabbitMqTracePropagation.Inject(activity, properties);
            }

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _options.QueueName,
                basicProperties: properties,
                body: body);

            activity?.SetStatus(ActivityStatusCode.Ok);

            return Task.FromResult(true);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            _logger.LogWarning(
                exception,
                "Failed to enqueue audit log {Action} for {ServiceName}",
                enrichedRequest.Action,
                enrichedRequest.ServiceName);

            return Task.FromResult(false);
        }
    }
}
