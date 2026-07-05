using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace SecurityPlatform.BuildingBlocks.Audit;

public sealed class AuditLogClient : IAuditLogClient
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly AuditMessagingOptions _options;
    private readonly ILogger<AuditLogClient> _logger;

    public AuditLogClient(
        ConnectionFactory connectionFactory,
        IOptions<AuditMessagingOptions> options,
        ILogger<AuditLogClient> logger)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
        _logger = logger;
    }

    public Task<bool> TryWriteAsync(AuditLogWriteRequest request, CancellationToken cancellationToken)
    {
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

            var body = JsonSerializer.SerializeToUtf8Bytes(request, AuditSerialization.JsonOptions);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Type = nameof(AuditLogWriteRequest);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _options.QueueName,
                basicProperties: properties,
                body: body);

            return Task.FromResult(true);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to enqueue audit log {Action} for {ServiceName}",
                request.Action,
                request.ServiceName);

            return Task.FromResult(false);
        }
    }
}
