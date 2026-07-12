using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SecurityAudit.Api.Features.AuditLogs.Abstractions;
using SecurityPlatform.BuildingBlocks.Audit;
using SecurityPlatform.BuildingBlocks.Diagnostics.Metrics;

namespace SecurityAudit.Api.Infrastructure.Messaging;

public sealed class AuditLogQueueConsumer : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private readonly ConnectionFactory _connectionFactory;
    private readonly AuditMessagingOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditLogQueueConsumer> _logger;
    private readonly SecurityPlatformMetricStore _metricStore;
    private IConnection? _connection;
    private IModel? _channel;

    public AuditLogQueueConsumer(
        ConnectionFactory connectionFactory,
        IOptions<AuditMessagingOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<AuditLogQueueConsumer> logger,
        SecurityPlatformMetricStore metricStore)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _metricStore = metricStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunConsumerAsync(stoppingToken);
                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (BrokerUnreachableException exception)
            {
                _logger.LogWarning(exception, "RabbitMQ is not ready yet. Retrying in 5 seconds.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RabbitMQ consumer stopped unexpectedly. Retrying in 5 seconds.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private Task RunConsumerAsync(CancellationToken stoppingToken)
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += HandleMessageAsync;

        _channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Listening for audit messages on queue {QueueName}.", _options.QueueName);

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (_channel is null)
        {
            return;
        }

        ActivityContext parentContext = default;
        if (eventArgs.BasicProperties is not null &&
            RabbitMqTracePropagation.TryExtract(eventArgs.BasicProperties, out var extractedContext))
        {
            parentContext = extractedContext;
        }

        using var activity = parentContext == default
            ? SecurityPlatformActivitySource.ActivitySource.StartActivity("Consume audit log", ActivityKind.Consumer)
            : SecurityPlatformActivitySource.ActivitySource.StartActivity(
                "Consume audit log",
                ActivityKind.Consumer,
                parentContext);
        using var initialLogScope = SecurityPlatformLogContext.Push(eventArgs.BasicProperties?.CorrelationId);

        try
        {
            var startedAt = Stopwatch.GetTimestamp();
            var message = JsonSerializer.Deserialize<AuditLogWriteRequest>(eventArgs.Body.Span, JsonOptions);
            if (message is null)
            {
                _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                _metricStore.RecordAuditLogConsumeFailure();
                return;
            }

            using var messageLogScope = SecurityPlatformLogContext.Push(message.CorrelationId);

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                OccurredAt = message.OccurredAt ?? DateTimeOffset.UtcNow,
                ServiceName = message.ServiceName.Trim(),
                Action = message.Action.Trim(),
                Level = message.Level,
                Details = message.Details.Trim(),
                ResourceType = message.ResourceType?.Trim(),
                ResourceId = message.ResourceId?.Trim(),
                ActorId = message.ActorId?.Trim(),
                ActorName = message.ActorName?.Trim(),
                CorrelationId = message.CorrelationId?.Trim(),
                MetadataJson = message.MetadataJson?.Trim()
            };

            await repository.AddAsync(auditLog, CancellationToken.None);
            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);

            _logger.LogInformation(
                "Stored audit event {Action} from {ServiceName}.",
                message.Action,
                message.ServiceName);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination", _options.QueueName);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _metricStore.RecordAuditLogConsumed(Stopwatch.GetElapsedTime(startedAt));
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            _logger.LogError(exception, "Failed to process audit log message.");
            _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
            _metricStore.RecordAuditLogConsumeFailure();
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        return base.StopAsync(cancellationToken);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
