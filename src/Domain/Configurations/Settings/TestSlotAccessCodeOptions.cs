using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class TestSlotAccessCodeOptions
{
    public const string SectionName = "TestSlotAccessCode";

    [Required]
    public required string EncryptionKey { get; init; }
}
