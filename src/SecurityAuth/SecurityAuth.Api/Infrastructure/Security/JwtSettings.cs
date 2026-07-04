namespace SecurityAuth.Api.Infrastructure.Security;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = "SecurityOperationsPlatform";
    public string Audience { get; set; } = "SecurityOperationsPlatform";
    public string Key { get; set; } = "CHANGE-ME-IN-PRODUCTION-THIS-IS-ONLY-FOR-LOCAL-DEVELOPMENT-KEY";
    public int ExpirationMinutes { get; set; } = 120;
}
