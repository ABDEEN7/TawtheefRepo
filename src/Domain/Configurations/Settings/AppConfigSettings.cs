using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class AppConfigSettings
{
    public const string SectionName = "AppConfig";
    [Required]
    public required string FrontendUrl { get; init; }
    [Required]
    public required string BackendUrl { get; init; }
    [Required]
    public required string BlobSignKey { get; init; }
    [Required]
    public required string? AdminEmail { get; init; }
    [Required, MinLength(1)]
    public required string[] AdminEmails { get; init; }
    public required int DefaultSignedUrlMinutes { get; init; } = 3;
}
