namespace Tawtheef.Domain.Configurations.Settings;

public sealed class StorageSettings
{
    public required string Provider { get; init; }
    public required string RootPath { get; init; }
    public required string PublicBaseUrl { get; init; }
    public string? AzureConnectionString { get; init; }
}
