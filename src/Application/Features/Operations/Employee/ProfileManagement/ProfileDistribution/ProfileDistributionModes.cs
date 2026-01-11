namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution;

public static class ProfileDistributionModes
{
    public const string Auto = "Auto";
    public const string Manual = "Manual";
    public const string AutoNormalized = "auto";
    public const string ManualNormalized = "manual";

    public static string Normalize(string mode) => mode.Trim().ToLowerInvariant();
}
