using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;

internal static class LanguagePointsCalculator
{
    private const string SpeakingMaxCode = "speaking.max";
    private const string ReadingMaxCode = "reading.max";
    private const string ConversationMaxCode = "conversation.max";
    private const string NativeCode = "native";

    private const string Excellent = "Excellent";
    private const string VeryGood = "veryGood";
    private const string Good = "good";

    public static int Calculate(ICollection<ProfileLanguage>? languages, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Language, IsDeleted: false })
            .ToList();

        if (details.Count == 0 || languages == null)
            return 0;

        var speakingPoints = SumLanguageAbilityPoints(
            languages, details, "speaking",
            language => language.SpeakingLevelId,
            SpeakingMaxCode);

        var readingPoints = SumLanguageAbilityPoints(
            languages, details, "reading",
            language => language.ReadingLevelId,
            ReadingMaxCode);

        var conversationPoints = SumLanguageAbilityPoints(
            languages, details, "conversation",
            language => language.WritingLevelId,
            ConversationMaxCode);

        var nativePoints = 0;
        var hasNative = languages.Any(language =>
            language.SpeakingLevelId == LanguageLevelIds.Native ||
            language.ReadingLevelId == LanguageLevelIds.Native ||
            language.WritingLevelId == LanguageLevelIds.Native);

        if (hasNative)
            nativePoints = JobPointsHelpers.GetDetailPoints(details, NativeCode);

        var total = speakingPoints + readingPoints + conversationPoints + nativePoints;
        return JobPointsHelpers.Clamp(total, jobPoints.Languages);
    }

    private static int SumLanguageAbilityPoints(
        IEnumerable<ProfileLanguage> languages,
        IReadOnlyCollection<JobPointsDetail> details,
        string abilityKey,
        Func<ProfileLanguage, Guid> levelSelector,
        string maxCode)
    {
        var maxPoints = JobPointsHelpers.GetDetailPoints(details, maxCode);
        var sum = languages.Select(language => MapLanguageLevel(levelSelector(language))).OfType<string>().Sum(levelCode => JobPointsHelpers.GetDetailPoints(details, $"{abilityKey}.{levelCode}"));

        return maxPoints > 0 ? Math.Min(sum, maxPoints) : sum;
    }

    private static string? MapLanguageLevel(Guid levelId)
    {
        if (levelId == LanguageLevelIds.Native)
            return null;

        if (levelId == LanguageLevelIds.Expert)
            return Excellent;

        if (levelId == LanguageLevelIds.Advanced)
            return VeryGood;

        if (levelId == LanguageLevelIds.Intermediate || levelId == LanguageLevelIds.Basic)
            return Good;

        return null;
    }
}
