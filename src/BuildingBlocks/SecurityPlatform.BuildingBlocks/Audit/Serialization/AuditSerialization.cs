using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecurityPlatform.BuildingBlocks.Audit;

public static class AuditSerialization
{
    public static JsonSerializerOptions JsonOptions { get; } = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
