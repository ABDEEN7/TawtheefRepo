namespace Tawtheef.Domain.Configurations.Settings;

public sealed class StorageSettings
{
    public required string RootPath { get; init; }
    public required string PublicBaseUrl { get; init; }
}
