using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobCandidates.Utilities;

internal static class ExperiencePointsCalculator
{
    private const string PointsPerYearCode = "pointsPerYear";
    private const string MaxYearsCode = "maxYears";

    public static int Calculate(double experiences, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Experience, IsDeleted: false })
            .ToList();

        var pointsPerYear = JobPointsHelpers.GetDetailPoints(details, PointsPerYearCode);
        var maxYears = JobPointsHelpers.GetDetailPoints(details, MaxYearsCode);
        if (pointsPerYear <= 0 || maxYears <= 0)
            return 0;
        var eligibleYears = Math.Min(experiences, maxYears);

        var points = eligibleYears * pointsPerYear;
        return JobPointsHelpers.Clamp((int)points, jobPoints.Experience);
    }
}
