namespace Tawtheef.Domain.Configurations;

public class QatarPassAuthSettings
{
    public const string SectionName = "Authentication:QatarPass:BaseUrl";
    public required string BaseUrl { get; init; }
}
