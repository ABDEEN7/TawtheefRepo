namespace Tawtheef.Domain.Configurations.Settings;

public sealed class GraphEmailSettings
{
    public static readonly string SectionName = "GraphEmail";
    public required string TenantId { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string FromAddress { get; init; }
    public bool SaveToSentItems { get; init; } = true;
    public int TimeoutSeconds { get; set; } = 60;

    // optional: logo
    public string? LogoPath { get; init; }
    public string? LogoUrl { get; init; }
    public string? FrontendBaseUrl { get; init; } // or reuse AppConfigSettings.FrontendUrl
}
