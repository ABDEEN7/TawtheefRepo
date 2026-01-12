using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class GoogleAuthenticationSettings
{
    public const string SectionName = "Authentication:Google";
    [Required]
    public required string ClientId { get; init; }
    [Required]
    public required string ClientSecret { get; init; }
}