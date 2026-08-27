using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public class MoiSettings
{
    public const string SectionName = "Moi";
    [Required]
    public required string BaseUrl { get; init; }
    [Required]
    public required string Username { get; init; }
    [Required]
    public required string Password { get; init; }
    public required int TimeoutSeconds { get; init; } = 30;
}
