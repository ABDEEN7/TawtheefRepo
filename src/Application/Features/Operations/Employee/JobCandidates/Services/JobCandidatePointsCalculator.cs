using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;

internal sealed class JobCandidatePointsCalculator
{
    private const string PointsPerYearCode = "pointsPerYear";
    private const string MaxYearsCode = "maxYears";
    private const string TrainingHighCode = "highLinked";
    private const string TrainingMediumCode = "mediumLinked";
    private const string TrainingLowCode = "lowLinked";
    private const string CertificatesLinkedCode = "certificatesLinked";
    private const string CertificatesNotLinkedCode = "certificatesNotLinked";
    private const string PrizeCode = "prize";
    private const string SpeakingMaxCode = "speaking.max";
    private const string ReadingMaxCode = "reading.max";
    private const string ConversationMaxCode = "conversation.max";
    private const string NativeCode = "native";

    private static readonly IReadOnlyDictionary<string, string> CandidateTypeCodeMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(CandidateTypeIds.Qatari)] = "qatari",
            [nameof(CandidateTypeIds.GCC)] = "gcc",
            [nameof(CandidateTypeIds.SonOfQatariMother)] = "qatarMother"
        };

    public int Calculate(JobCandidateRecord candidate, JobPointsMain? jobPoints)
    {
        if (jobPoints == null || candidate.Profile == null)
        {
            return 0;
        }

        var applicantCategoryPoints = CalculateApplicantCategoryPoints(candidate.Profile, jobPoints);
        var educationPoints = CalculateEducationPoints(candidate.Profile, jobPoints);
        var experiencePoints = CalculateExperiencePoints(candidate.Profile, jobPoints);
        var trainingPoints = CalculateTrainingPoints(candidate.Profile, jobPoints);
        var skillsPoints = CalculateSkillPoints(candidate.Profile, jobPoints);
        var languagePoints = CalculateLanguagePoints(candidate.Profile, jobPoints);
        var certificatesPoints = CalculateCertificatesPoints(candidate.Profile, jobPoints);

        return applicantCategoryPoints
            + educationPoints
            + experiencePoints
            + trainingPoints
            + skillsPoints
            + languagePoints
            + certificatesPoints;
    }

    private static int CalculateApplicantCategoryPoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.ApplicantCategory && !detail.IsDeleted)
            .ToList();
        if (details.Count == 0)
        {
            return 0;
        }

        var matchingCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var candidateType = profile.CandidateType?.BackendName;
        if (!string.IsNullOrWhiteSpace(candidateType))
        {
            if (CandidateTypeCodeMap.TryGetValue(candidateType, out var mappedCode))
            {
                matchingCodes.Add(mappedCode);
            }

            matchingCodes.Add(candidateType);
        }

        var qatarQualifications = profile.Qualifications?
            .Where(q => IsQatarUniversity(q.University))
            .ToList() ?? [];

        if (qatarQualifications.Count > 0)
        {
            matchingCodes.Add("qatarGraduate");
        }

        if (qatarQualifications.Count > 1)
        {
            matchingCodes.Add("qatarGraduatePrev");
        }

        var points = details
            .Where(detail => matchingCodes.Contains(detail.Code))
            .Sum(detail => detail.Points);

        return Clamp(points, jobPoints.ApplicantCategory);
    }

    private static int CalculateEducationPoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.Education && !detail.IsDeleted)
            .ToList();
        if (details.Count == 0 || profile.Qualifications == null)
        {
            return 0;
        }

        var maxPoints = details
            .Where(detail => profile.Qualifications.Any(q => MatchesEducationDetail(q, detail)))
            .Select(detail => detail.Points)
            .DefaultIfEmpty(0)
            .Max();

        return Clamp(maxPoints, jobPoints.Education);
    }

    private static int CalculateExperiencePoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.Experience && !detail.IsDeleted)
            .ToList();

        var pointsPerYear = GetDetailPoints(details, PointsPerYearCode);
        var maxYears = GetDetailPoints(details, MaxYearsCode);
        if (pointsPerYear <= 0 || maxYears <= 0)
        {
            return 0;
        }

        var totalYears = CalculateExperienceYears(profile.Experiences);
        var eligibleYears = Math.Min(totalYears, maxYears);
        var points = eligibleYears * pointsPerYear;

        return Clamp(points, jobPoints.Experience);
    }

    private static int CalculateTrainingPoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.Training && !detail.IsDeleted)
            .ToList();

        if (details.Count == 0 || profile.TrainingCourses == null)
        {
            return 0;
        }

        var points = 0;
        foreach (var training in profile.TrainingCourses)
        {
            var detailCode = training.SpecializationRelation switch
            {
                SpecializationRelationLevel.Strong => TrainingHighCode,
                SpecializationRelationLevel.Medium => TrainingMediumCode,
                SpecializationRelationLevel.Weak => TrainingLowCode,
                _ => null
            };

            if (detailCode == null)
            {
                continue;
            }

            points += GetDetailPoints(details, detailCode);
        }

        return Clamp(points, jobPoints.Training);
    }

    private static int CalculateSkillPoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.Skill && !detail.IsDeleted)
            .ToList();

        if (details.Count == 0 || profile.Skills == null)
        {
            return 0;
        }

        var skillIds = profile.Skills
            .Select(skill => skill.SkillId)
            .ToHashSet();

        var points = details
            .Where(detail =>
                (detail.ReferenceId.HasValue && skillIds.Contains(detail.ReferenceId.Value)) ||
                profile.Skills.Any(skill =>
                    skill.Skill?.BackendName != null &&
                    string.Equals(skill.Skill.BackendName, detail.Code, StringComparison.OrdinalIgnoreCase)))
            .Sum(detail => detail.Points);

        return Clamp(points, jobPoints.Skills);
    }

    private static int CalculateLanguagePoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.Language && !detail.IsDeleted)
            .ToList();

        if (details.Count == 0 || profile.Languages == null)
        {
            return 0;
        }

        var speakingPoints = SumLanguageAbilityPoints(profile.Languages, details, "speaking",
            language => language.SpeakingLevelId,
            SpeakingMaxCode);
        var readingPoints = SumLanguageAbilityPoints(profile.Languages, details, "reading",
            language => language.ReadingLevelId,
            ReadingMaxCode);
        var conversationPoints = SumLanguageAbilityPoints(profile.Languages, details, "conversation",
            language => language.WritingLevelId,
            ConversationMaxCode);

        var nativePoints = 0;
        var hasNative = profile.Languages.Any(language =>
            language.SpeakingLevelId == LanguageLevelIds.Native ||
            language.ReadingLevelId == LanguageLevelIds.Native ||
            language.WritingLevelId == LanguageLevelIds.Native);
        if (hasNative)
        {
            nativePoints = GetDetailPoints(details, NativeCode);
        }

        var total = speakingPoints + readingPoints + conversationPoints + nativePoints;
        return Clamp(total, jobPoints.Languages);
    }

    private static int CalculateCertificatesPoints(UserProfile profile, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(detail => detail.Type == JobPointRuleType.Certificate && !detail.IsDeleted)
            .ToList();

        if (details.Count == 0 || profile.Achievements == null)
        {
            return 0;
        }

        var points = 0;
        foreach (var achievement in profile.Achievements)
        {
            if (achievement.AchievementTypeId == AchievementTypeIds.Certificate)
            {
                points += achievement.RelatedToSpecialization == true
                    ? GetDetailPoints(details, CertificatesLinkedCode)
                    : GetDetailPoints(details, CertificatesNotLinkedCode);
            }
            else if (achievement.AchievementTypeId == AchievementTypeIds.Award)
            {
                points += GetDetailPoints(details, PrizeCode);
            }
        }

        return Clamp(points, jobPoints.Certificates);
    }

    private static int CalculateExperienceYears(ICollection<Experience>? experiences)
    {
        if (experiences == null || experiences.Count == 0)
        {
            return 0;
        }

        var totalMonths = 0;
        foreach (var experience in experiences)
        {
            var start = experience.StartDate;
            var end = experience.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            if (end < start)
            {
                continue;
            }

            var months = (end.Year - start.Year) * 12 + (end.Month - start.Month);
            if (end.Day < start.Day)
            {
                months = Math.Max(0, months - 1);
            }

            totalMonths += months;
        }

        return totalMonths / 12;
    }

    private static bool MatchesEducationDetail(Qualification qualification, JobPointsDetail detail)
    {
        if (detail.ReferenceId.HasValue && qualification.DegreeId == detail.ReferenceId.Value)
        {
            return true;
        }

        return qualification.Degree?.BackendName != null &&
               string.Equals(qualification.Degree.BackendName, detail.Code, StringComparison.OrdinalIgnoreCase);
    }

    private static int SumLanguageAbilityPoints(
        IEnumerable<ProfileLanguage> languages,
        IReadOnlyCollection<JobPointsDetail> details,
        string abilityKey,
        Func<ProfileLanguage, Guid> levelSelector,
        string maxCode)
    {
        var maxPoints = GetDetailPoints(details, maxCode);
        var sum = 0;

        foreach (var language in languages)
        {
            var levelCode = MapLanguageLevel(levelSelector(language));
            if (levelCode == null)
            {
                continue;
            }

            sum += GetDetailPoints(details, $"{abilityKey}.{levelCode}");
        }

        return maxPoints > 0 ? Math.Min(sum, maxPoints) : sum;
    }

    private static string? MapLanguageLevel(Guid levelId)
    {
        if (levelId == LanguageLevelIds.Expert)
        {
            return "excellent";
        }

        if (levelId == LanguageLevelIds.Advanced)
        {
            return "veryGood";
        }

        if (levelId == LanguageLevelIds.Intermediate || levelId == LanguageLevelIds.Basic)
        {
            return "good";
        }

        if (levelId == LanguageLevelIds.Native)
        {
            return "excellent";
        }

        return null;
    }

    private static int GetDetailPoints(IEnumerable<JobPointsDetail> details, string code)
    {
        return details
            .Where(detail => string.Equals(detail.Code, code, StringComparison.OrdinalIgnoreCase))
            .Select(detail => detail.Points)
            .FirstOrDefault();
    }

    private static bool IsQatarUniversity(University? university)
    {
        if (university == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(university.BackendName) &&
            string.Equals(university.BackendName, "qatarUniversity", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(university.NameEn) &&
            university.NameEn.Contains("Qatar University", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(university.NameAr) &&
               university.NameAr.Contains("جامعة قطر", StringComparison.OrdinalIgnoreCase);
    }

    private static int Clamp(int value, int max)
    {
        if (max <= 0)
        {
            return 0;
        }

        return value > max ? max : value;
    }
}
