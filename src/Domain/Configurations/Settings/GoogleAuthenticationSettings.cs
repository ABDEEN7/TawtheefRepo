using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class GoogleAuthenticationSettings
{
    public const string SectionName = "Authentication:Google";
    public bool IsEnabled { get; init; } = false;
    [Required]
    public required string ClientId { get; init; }
    [Required]
    public required string ClientSecret { get; init; }
}
