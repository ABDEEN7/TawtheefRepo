using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public enum StorageProvider
{
    Local,
    AzureBlobStorage
}
public sealed class StorageSettings
{
    public const string SectionName = "Storage";
    [Required]
    public required string Provider { get; init; }
    [Required]
    public required string RootPath { get; init; }
    [Required]
    public required string PublicBaseUrl { get; init; }
    public string? AzureConnectionString { get; init; }
}
