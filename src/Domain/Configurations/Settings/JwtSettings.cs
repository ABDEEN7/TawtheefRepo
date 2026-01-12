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
    public JwtValidationSettings TokenValidationParameters { get; init; } = new();
}

public sealed class JwtValidationSettings
{
    public bool ValidateIssuer { get; init; } = true;
    public bool ValidateAudience { get; init; } = true;
    public bool ValidateLifetime { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
    public bool RequireExpirationTime { get; init; } = true;
    public TimeSpan ClockSkew { get; init; } = TimeSpan.FromMinutes(1);
}
