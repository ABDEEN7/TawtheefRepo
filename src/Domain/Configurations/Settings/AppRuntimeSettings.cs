namespace Tawtheef.Domain.Configurations.Settings;
public enum AppModule
{
    Recruitment = 1,
    Operation = 2
}
public sealed class AppRuntimeSettings
{
    public const string SectionName = "App";
    public AppModule Module { get; set; }
}
