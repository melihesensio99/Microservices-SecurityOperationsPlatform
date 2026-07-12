using System.Globalization;
using System.Text;
using System.Threading;

namespace SecurityPlatform.BuildingBlocks.Diagnostics.Metrics;

public sealed class SecurityPlatformMetricStore
{
    private long _auditLogsPublishedTotal;
    private long _auditLogsPublishFailuresTotal;
    private long _auditLogsConsumedTotal;
    private long _auditLogsConsumeFailuresTotal;
    private long _auditLogConsumeDurationTotalMilliseconds;
    private long _auditLogConsumeDurationCount;

    public void RecordAuditLogPublished() =>
        Interlocked.Increment(ref _auditLogsPublishedTotal);

    public void RecordAuditLogPublishFailure() =>
        Interlocked.Increment(ref _auditLogsPublishFailuresTotal);

    public void RecordAuditLogConsumed(TimeSpan duration)
    {
        Interlocked.Increment(ref _auditLogsConsumedTotal);
        Interlocked.Add(ref _auditLogConsumeDurationTotalMilliseconds, (long)duration.TotalMilliseconds);
        Interlocked.Increment(ref _auditLogConsumeDurationCount);
    }

    public void RecordAuditLogConsumeFailure() =>
        Interlocked.Increment(ref _auditLogsConsumeFailuresTotal);

    public string ExportPrometheus()
    {
        var published = Interlocked.Read(ref _auditLogsPublishedTotal);
        var publishFailures = Interlocked.Read(ref _auditLogsPublishFailuresTotal);
        var consumed = Interlocked.Read(ref _auditLogsConsumedTotal);
        var consumeFailures = Interlocked.Read(ref _auditLogsConsumeFailuresTotal);
        var durationTotal = Interlocked.Read(ref _auditLogConsumeDurationTotalMilliseconds);
        var durationCount = Interlocked.Read(ref _auditLogConsumeDurationCount);
        var averageDuration = durationCount == 0
            ? 0
            : (double)durationTotal / durationCount;

        var builder = new StringBuilder();
        AppendCounter(builder, "security_platform_audit_logs_published_total", "Total audit logs published successfully.", published);
        AppendCounter(builder, "security_platform_audit_logs_publish_failures_total", "Total audit log publish failures.", publishFailures);
        AppendCounter(builder, "security_platform_audit_logs_consumed_total", "Total audit logs consumed successfully.", consumed);
        AppendCounter(builder, "security_platform_audit_logs_consume_failures_total", "Total audit log consume failures.", consumeFailures);
        AppendGauge(builder, "security_platform_audit_log_consume_duration_ms_avg", "Average audit log consume duration in milliseconds.", averageDuration);

        return builder.ToString();
    }

    private static void AppendCounter(StringBuilder builder, string name, string help, long value)
    {
        builder.AppendLine($"# HELP {name} {help}");
        builder.AppendLine($"# TYPE {name} counter");
        builder.AppendLine($"{name} {value.ToString(CultureInfo.InvariantCulture)}");
    }

    private static void AppendGauge(StringBuilder builder, string name, string help, double value)
    {
        builder.AppendLine($"# HELP {name} {help}");
        builder.AppendLine($"# TYPE {name} gauge");
        builder.AppendLine($"{name} {value.ToString(CultureInfo.InvariantCulture)}");
    }
}
