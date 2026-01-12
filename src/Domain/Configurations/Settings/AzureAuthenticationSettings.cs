using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class AzureAuthenticationSettings
{
    public const string SectionName = "Authentication:Azure";
    [Required]
    public required string ClientId { get; init; }
    [Required]
    public required string TenantId { get; init; }
    [Required]
    public required string Instance { get; init; }

    public TimeSpan ClockSkew { get; init; } = TimeSpan.FromMinutes(2);
}
