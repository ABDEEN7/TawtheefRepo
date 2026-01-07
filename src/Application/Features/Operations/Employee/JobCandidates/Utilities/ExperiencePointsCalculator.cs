using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;

internal static class ExperiencePointsCalculator
{
    private const string PointsPerYearCode = "pointsPerYear";
    private const string MaxYearsCode = "maxYears";

    public static int Calculate(ICollection<Experience>? experiences, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Experience, IsDeleted: false })
            .ToList();

        var pointsPerYear = JobPointsHelpers.GetDetailPoints(details, PointsPerYearCode);
        var maxYears = JobPointsHelpers.GetDetailPoints(details, MaxYearsCode);
        if (pointsPerYear <= 0 || maxYears <= 0)
            return 0;

        var totalYears = CalculateExperienceYears(experiences);
        var eligibleYears = Math.Min(totalYears, maxYears);

        var points = eligibleYears * pointsPerYear;
        return JobPointsHelpers.Clamp(points, jobPoints.Experience);
    }

    private static int CalculateExperienceYears(ICollection<Experience>? experiences)
    {
        if (experiences == null || experiences.Count == 0)
            return 0;

        var totalMonths = 0;

        foreach (var experience in experiences)
        {
            var start = experience.StartDate;
            var end = experience.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            if (end < start) continue;

            var months = (end.Year - start.Year) * 12 + (end.Month - start.Month);
            if (end.Day < start.Day) months = Math.Max(0, months - 1);

            totalMonths += months;
        }

        return totalMonths / 12;
    }
}
