using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class RecaptchaSettings
{
    public const string SectionName = "RecaptchaSettings";
    [Required]
    public required string BaseUrl { get; init; }
    [Required]
    public required string SecretKey { get; init; }
    [Required]
    public required string SiteKey { get; init; }
    [Required]
    public required string Version { get; init; }
    public required int TimeoutSeconds { get; init; } = 10;
}
