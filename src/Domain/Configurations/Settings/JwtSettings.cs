using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "Authentication:Jwt";
    [Required]
    public required string SigningKey { get; init; }
    [Required]
    public required string Issuer { get; init; }
    [Required]
    public required string Audience { get; init; }
    public int? ExpiryMinutes { get; set; }
    public int? RefreshTokenExpirationDays { get; set; }
    public int TokenLifetimeMinutes { get; init; }
    public int? RefreshTokenRetentionCount { get; init; }
}

public sealed class GoogleAuthenticationSettings
{
    public const string SectionName = "Authentication:Google";
    [Required]
    public required string ClientId { get; init; }
    [Required]
    public required string ClientSecret { get; init; }
}


public sealed class AzureAuthenticationSettings
{
    public const string SectionName = "Authentication:Azure";
    [Required]
    public required string ClientId { get; init; }
    [Required]
    public required string TenantId { get; init; }
    [Required]
    public required string Instance { get; init; }
}
