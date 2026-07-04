using Microsoft.AspNetCore.Http;

namespace SecurityPlatform.BuildingBlocks.Results;

public sealed record Error(string Code, string Message, int StatusCode = StatusCodes.Status400BadRequest)
{
    public static Error None { get; } = new(string.Empty, string.Empty, StatusCodes.Status200OK);
}
