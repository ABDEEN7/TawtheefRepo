using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class TestSlotSettings
{
    public const string SectionName = "TestSlot";

    [Range(0, 1440)]
    public int StartAllowedBeforeMinutes { get; init; } = 60;
}
