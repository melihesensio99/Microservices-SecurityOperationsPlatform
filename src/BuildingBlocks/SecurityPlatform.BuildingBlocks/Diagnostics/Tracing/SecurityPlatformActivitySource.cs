using System.Diagnostics;

namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public static class SecurityPlatformActivitySource
{
    public const string SourceName = "SecurityPlatform";
    public static readonly ActivitySource ActivitySource = new(SourceName);

    static SecurityPlatformActivitySource()
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
    }
}
