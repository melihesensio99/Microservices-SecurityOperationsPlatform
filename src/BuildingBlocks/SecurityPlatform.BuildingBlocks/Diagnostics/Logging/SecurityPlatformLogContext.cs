using System.Diagnostics;
using Serilog.Context;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public static class SecurityPlatformLogContext
{
    public static IDisposable Push(
        string? correlationId = null,
        string? serviceName = null)
    {
        var disposables = new List<IDisposable>();

        if (!string.IsNullOrWhiteSpace(serviceName))
        {
            disposables.Add(LogContext.PushProperty("ServiceName", serviceName));
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            disposables.Add(LogContext.PushProperty("CorrelationId", correlationId));
        }

        var activity = Activity.Current;
        if (activity is not null)
        {
            disposables.Add(LogContext.PushProperty("TraceId", activity.TraceId.ToString()));
            disposables.Add(LogContext.PushProperty("SpanId", activity.SpanId.ToString()));
        }

        return new CompositeDisposable(disposables);
    }

    private sealed class CompositeDisposable : IDisposable
    {
        private readonly IReadOnlyList<IDisposable> _disposables;

        public CompositeDisposable(IReadOnlyList<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            for (var index = _disposables.Count - 1; index >= 0; index--)
            {
                _disposables[index].Dispose();
            }
        }
    }
}
