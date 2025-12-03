namespace Tawtheef.Domain.Configurations.Settings;

public class MoiSettings
{
    public const string SectionName = "Moi";
    public required string BaseUrl { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}
