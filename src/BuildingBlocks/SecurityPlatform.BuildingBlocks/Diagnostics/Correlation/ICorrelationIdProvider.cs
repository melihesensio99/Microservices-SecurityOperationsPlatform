namespace SecurityPlatform.BuildingBlocks.Diagnostics;

public interface ICorrelationIdProvider
{
    string? GetCorrelationId();
}
