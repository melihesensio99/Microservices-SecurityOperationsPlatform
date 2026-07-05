namespace SecurityPlatform.BuildingBlocks.Audit;

public sealed class AuditMessagingOptions
{
    public string HostName { get; init; } = "localhost";

    public int Port { get; init; } = 5672;

    public string VirtualHost { get; init; } = "/";

    public string UserName { get; init; } = "securityplatform";

    public string Password { get; init; } = "securityplatform";

    public string QueueName { get; init; } = "security-platform.audit-log-events";
}
