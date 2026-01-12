using Application.Operation.Features.Employee.Job.DTOs;
using FluentResults;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.Job.Utilities;

internal static class JobPointsValidationUtility
{
    private readonly record struct JobPointsDetailSnapshot(
        JobPointRuleType Type,
        string Code,
        int Points);

    private readonly record struct JobPointsTotals(
        int ApplicantCategory,
        int Education,
        int Experience,
        int Training,
        int Skills,
        int Languages,
        int Certificates,
        int Total);

    public static Result Validate(JobPointsMainRequestDto dto, JobPointConfiguration config)
    {
        var totals = new JobPointsTotals(
            dto.ApplicantCategory,
            dto.Education,
            dto.Experience,
            dto.Training,
            dto.Skills,
            dto.Languages,
            dto.Certificates,
            dto.Total);

        var details = dto.Details.Select(detail => new JobPointsDetailSnapshot(
            detail.Type,
            detail.Code,
            detail.Points));

        return Validate(totals, details, config);
    }

    public static Result Validate(JobPointsMain points, JobPointConfiguration config)
    {
        var totals = new JobPointsTotals(
            points.ApplicantCategory,
            points.Education,
            points.Experience,
            points.Training,
            points.Skills,
            points.Languages,
            points.Certificates,
            points.Total);

        var details = points.Details.Select(detail => new JobPointsDetailSnapshot(
            detail.Type,
            detail.Code,
            detail.Points));

        return Validate(totals, details, config);
    }

    private static Result Validate(
        JobPointsTotals totals,
        IEnumerable<JobPointsDetailSnapshot> details,
        JobPointConfiguration config)
    {
        if (!AreTotalsNonNegative(totals))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsMainTotalValid(totals, config))
            return Result.Fail(JobMessages.JobPointsTotalNotValid);

        var detailsList = details.ToList();
        if (detailsList.Any(detail => detail.Points < 0))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsSectionValid(detailsList, JobPointRuleType.ApplicantCategory, totals.ApplicantCategory))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsSectionValid(detailsList, JobPointRuleType.Education, totals.Education))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsExperienceValid(detailsList, totals.Experience))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsSectionValid(detailsList, JobPointRuleType.Training, totals.Training))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsSectionValid(detailsList, JobPointRuleType.Skill, totals.Skills))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsLanguagesValid(detailsList, totals.Languages))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        if (!IsSectionValid(detailsList, JobPointRuleType.Certificate, totals.Certificates))
            return Result.Fail(JobMessages.JobPointsDetailsNotValid);

        return Result.Ok();
    }

    private static bool IsMainTotalValid(JobPointsTotals totals, JobPointConfiguration config)
    {
        var sum = totals.ApplicantCategory +
                  totals.Education +
                  totals.Experience +
                  totals.Training +
                  totals.Skills +
                  totals.Languages +
                  totals.Certificates;

        if (sum != totals.Total)
            return false;

        if (totals.Total != config.MaxPoints)
            return false;

        return totals.ApplicantCategory <= config.ApplicantCategoryMaxPoints &&
               totals.Education <= config.EducationMaxPoints &&
               totals.Experience <= config.ExperienceMaxPoints &&
               totals.Training <= config.TrainingMaxPoints &&
               totals.Skills <= config.SkillsMaxPoints &&
               totals.Languages <= config.LanguagesMaxPoints &&
               totals.Certificates <= config.CertificatesMaxPoints;
    }

    private static bool AreTotalsNonNegative(JobPointsTotals totals) =>
        totals.ApplicantCategory >= 0 &&
        totals.Education >= 0 &&
        totals.Experience >= 0 &&
        totals.Training >= 0 &&
        totals.Skills >= 0 &&
        totals.Languages >= 0 &&
        totals.Certificates >= 0 &&
        totals.Total >= 0;

    private static bool IsSectionValid(
        IReadOnlyCollection<JobPointsDetailSnapshot> details,
        JobPointRuleType type,
        int mainValue)
    {
        var total = details
            .Where(detail => detail.Type == type)
            .Sum(detail => detail.Points);

        return total <= mainValue;
    }

    private static bool IsExperienceValid(
        IReadOnlyCollection<JobPointsDetailSnapshot> details,
        int mainValue)
    {
        var pointsPerYear = GetDetailPoints(details, JobPointRuleType.Experience, "pointsPerYear");
        var maxYears = GetDetailPoints(details, JobPointRuleType.Experience, "maxYears");
        var total = pointsPerYear * maxYears;

        return total <= mainValue;
    }

    private static bool IsLanguagesValid(
        IReadOnlyCollection<JobPointsDetailSnapshot> details,
        int mainValue)
    {
        var languageDetails = details.Where(detail => detail.Type == JobPointRuleType.Language).ToList();
        var total = 0;

        foreach (var ability in new[] { "speaking", "reading", "conversation" })
        {
            var abilityMax = GetDetailPoints(languageDetails, JobPointRuleType.Language, $"{ability}.max");
            var levelsSum = languageDetails
                .Where(detail =>
                    detail.Code.StartsWith($"{ability}.", StringComparison.OrdinalIgnoreCase) &&
                    !detail.Code.EndsWith(".max", StringComparison.OrdinalIgnoreCase))
                .Sum(detail => detail.Points);

            if (levelsSum > abilityMax)
                return false;

            total += abilityMax > 0 ? Math.Min(levelsSum, abilityMax) : levelsSum;
        }

        total += GetDetailPoints(languageDetails, JobPointRuleType.Language, "native");

        return total <= mainValue;
    }

    private static int GetDetailPoints(
        IEnumerable<JobPointsDetailSnapshot> details,
        JobPointRuleType type,
        string code)
    {
        var detail = details.FirstOrDefault(detail =>
            detail.Type == type &&
            string.Equals(detail.Code, code, StringComparison.OrdinalIgnoreCase));

        return detail.Code == null ? 0 : detail.Points;
    }
}