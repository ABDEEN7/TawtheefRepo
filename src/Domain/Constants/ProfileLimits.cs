namespace Tawtheef.Domain.Constants;

public static class ProfileLimits
{
    public const int ExperienceDescriptionMaxLength = 2000;
    public const int TrainingDescriptionMaxLength = 2000;
    public const long MaxExperienceFileSizeBytes = 1_000_000;
    public const long MaxTrainingFileSizeBytes = 1_000_000;
    public const string MaxFileSizeLabel = "1MB";
}
