using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public class QatarPassAuthSettings
{
    public const string SectionName = "Authentication:QatarPass";
    [Required]
    public required string BaseUrl { get; init; }
    public required int TimeoutSeconds { get; init; } = 10;
}
