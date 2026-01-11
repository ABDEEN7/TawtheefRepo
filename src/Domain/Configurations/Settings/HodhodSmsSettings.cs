using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public class HodhodSmsSettings
{
    public const string SectionName = "HodhodSms";
    [Required]
    public required string BaseUrl { get; init; }
    [Required]
    public required string ApplicationId { get; init; }
    [Required]
    public required string Password { get; init; }
    public int DefaultOtpMinutes { get; init; } = 5;
    public bool ConfirmDelivery { get; init; } = true;
    public int Priority { get; init; } = 0;
    public required int TimeoutSeconds { get; init; } = 10;
}
