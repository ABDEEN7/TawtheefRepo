namespace Tawtheef.Domain.Configurations;

public sealed class RecaptchaSettings
{
    public required string SecretKey { get; init; }
    public required string SiteKey { get; init; }
    public required string Version { get; init; }
}