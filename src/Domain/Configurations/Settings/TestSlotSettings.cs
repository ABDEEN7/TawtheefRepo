using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class TestSlotSettings
{
    public const string SectionName = "TestSlot";

    [Range(0, 1440)]
    public int StartAllowedBeforeMinutes { get; init; } = 60;

    [Range(1, 240)]
    public int MinimumSessionEntryWindowMinutes { get; init; } = 10;

    [Range(5, 3600)]
    public int NoShowSweepSeconds { get; init; } = 30;
}
