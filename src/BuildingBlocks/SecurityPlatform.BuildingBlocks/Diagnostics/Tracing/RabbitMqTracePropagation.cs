using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public static class RabbitMqTracePropagation
{
    private const string TraceParentHeader = "traceparent";
    private const string TraceStateHeader = "tracestate";

    public static void Inject(Activity? activity, IBasicProperties properties)
    {
        if (activity is null)
        {
            return;
        }

        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers[TraceParentHeader] = Encoding.UTF8.GetBytes(activity.Id ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(activity.TraceStateString))
        {
            properties.Headers[TraceStateHeader] = Encoding.UTF8.GetBytes(activity.TraceStateString);
        }
    }

    public static bool TryExtract(
        IBasicProperties properties,
        out ActivityContext parentContext)
    {
        parentContext = default;

        if (properties.Headers is null)
        {
            return false;
        }

        var traceParent = TryGetHeader(properties.Headers, TraceParentHeader);
        if (string.IsNullOrWhiteSpace(traceParent))
        {
            return false;
        }

        var traceState = TryGetHeader(properties.Headers, TraceStateHeader);
        return ActivityContext.TryParse(traceParent, traceState, out parentContext);
    }

    private static string? TryGetHeader(
        IDictionary<string, object> headers,
        string key)
    {
        if (!headers.TryGetValue(key, out var value) || value is null)
        {
            return null;
        }

        return value switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string text => text,
            _ => value.ToString()
        };
    }
}
