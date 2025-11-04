namespace Tawtheef.Domain.Configurations.Settings;

public class QatarPassAuthSettings
{
    public const string SectionName = "Authentication:QatarPass:BaseUrl";
    public required string BaseUrl { get; init; }
}
