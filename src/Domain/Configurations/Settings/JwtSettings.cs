namespace Tawtheef.Domain.Configurations.Settings;

public sealed class JwtSettings
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int? ExpiryMinutes { get; set; }
    public int? RefreshTokenExpirationDays { get; set; }
    public int TokenLifetimeMinutes { get; init; }
    public int? RefreshTokenRetentionCount { get; init; }
}