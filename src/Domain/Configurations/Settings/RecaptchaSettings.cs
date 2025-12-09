namespace Tawtheef.Domain.Configurations.Settings;

public sealed class RecaptchaSettings
{
    public const string SectionName = "RecaptchaSettings";
    public required string BaseUrl { get; init; }
    public required string SecretKey { get; init; }
    public required string SiteKey { get; init; }
    public required string Version { get; init; }
    public required int TimeoutSeconds { get; init; } = 10;
}
