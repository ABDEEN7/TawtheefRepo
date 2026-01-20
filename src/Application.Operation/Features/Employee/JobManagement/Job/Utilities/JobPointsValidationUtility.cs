using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using FluentResults;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.Job.Utilities;

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
        if (detailsList.Any(detail => detail.Points < 0) ||
            !HasRequiredDetails(detailsList, totals) ||
            !IsSectionValid(detailsList, JobPointRuleType.ApplicantCategory, totals.ApplicantCategory) ||
            !IsSectionValid(detailsList, JobPointRuleType.Education, totals.Education) ||
            !IsExperienceValid(detailsList, totals.Experience) ||
            !IsSectionValid(detailsList, JobPointRuleType.Training, totals.Training) ||
            !IsSectionValid(detailsList, JobPointRuleType.Skill, totals.Skills) ||
            !IsLanguagesValid(detailsList, totals.Languages) ||
            !IsSectionValid(detailsList, JobPointRuleType.Certificate, totals.Certificates))
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

        return totals.Total == config.MaxPoints;
    }

    private static bool AreTotalsNonNegative(JobPointsTotals totals) =>
        totals.ApplicantCategory >= 0 &&
        totals is { Education: >= 0, Experience: >= 0, Training: >= 0, Skills: >= 0, Languages: >= 0, Certificates: >= 0, Total: >= 0 };

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

    private static bool HasRequiredDetails(
        IReadOnlyCollection<JobPointsDetailSnapshot> details,
        JobPointsTotals totals)
    {
        if (totals.ApplicantCategory > 0 && !HasSectionPoints(details, JobPointRuleType.ApplicantCategory))
            return false;

        if (totals.Education > 0 && !HasSectionPoints(details, JobPointRuleType.Education))
            return false;

        if (totals.Training > 0 && !HasSectionPoints(details, JobPointRuleType.Training))
            return false;

        if (totals.Skills > 0 && !HasSectionPoints(details, JobPointRuleType.Skill))
            return false;

        if (totals.Certificates > 0 && !HasSectionPoints(details, JobPointRuleType.Certificate))
            return false;

        if (totals.Experience > 0 && !HasExperienceDetails(details))
            return false;

        if (totals.Languages > 0 && !HasLanguageDetails(details))
            return false;

        return true;
    }

    private static bool HasSectionPoints(
        IReadOnlyCollection<JobPointsDetailSnapshot> details,
        JobPointRuleType type)
        => details.Any(detail => detail.Type == type && detail.Points > 0);

    private static bool HasExperienceDetails(IReadOnlyCollection<JobPointsDetailSnapshot> details)
    {
        var pointsPerYear = GetDetailPoints(details, JobPointRuleType.Experience, "pointsPerYear");
        var maxYears = GetDetailPoints(details, JobPointRuleType.Experience, "maxYears");
        return pointsPerYear > 0 && maxYears > 0;
    }

    private static bool HasLanguageDetails(IReadOnlyCollection<JobPointsDetailSnapshot> details)
    {
        var languageDetails = details
            .Where(detail => detail is { Type: JobPointRuleType.Language, Points: > 0 })
            .ToList();

        return languageDetails.Any(detail =>
            !detail.Code.EndsWith(".max", StringComparison.OrdinalIgnoreCase));
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
