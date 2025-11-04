namespace Tawtheef.Domain.Configurations.Settings;

public class HodhodSmsSettings
{
    public const string SectionName = "HodhodSms";
    public required string BaseUrl { get; init; }
    public required string ApplicationId { get; init; }
    public required string Password { get; init; }
    public int DefaultOtpMinutes { get; init; } = 5;
    public bool ConfirmDelivery { get; init; } = true;
    public int Priority { get; init; } = 0;
}
