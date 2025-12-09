namespace Tawtheef.Domain.Configurations.Settings;

public class QatarPassAuthSettings
{
    public const string SectionName = "Authentication:QatarPass";
    public required string BaseUrl { get; init; }
    public required int TimeoutSeconds { get; init; } = 10;
}
