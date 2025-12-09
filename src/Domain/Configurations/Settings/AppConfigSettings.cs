namespace Tawtheef.Domain.Configurations.Settings;

public sealed class AppConfigSettings
{
    public const string SectionName = "AppConfig";
    public required string FrontendUrl { get; init; }
    public required string BackendUrl { get; init; }
    public required string BlobSignKey { get; init; }
    public required string? AdminEmail { get; init; }
    public required string[] AdminEmails { get; init; }
    public required int DefaultSignedUrlMinutes { get; init; } = 3;
}
