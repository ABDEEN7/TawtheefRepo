using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobCandidates.Utilities;

internal static class ApplicantCategoryPointsCalculator
{
    public static int Calculate(CandidateType? candidateType, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.ApplicantCategory, IsDeleted: false })
            .ToList();

        if (details.Count == 0)
            return 0;

        var candidateTypeCode = candidateType?.BackendName;
        if (string.IsNullOrWhiteSpace(candidateTypeCode))
            return 0;

        var points = details
            .Where(d => string.Equals(d.Code, candidateTypeCode, StringComparison.OrdinalIgnoreCase))
            .Sum(d => d.Points);

        return JobPointsHelpers.Clamp(points, jobPoints.ApplicantCategory);
    }
}
