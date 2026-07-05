namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public sealed class TracingOptions
{
    public bool Enabled { get; set; } = true;

    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
}
