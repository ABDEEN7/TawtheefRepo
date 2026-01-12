using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobCandidates.Utilities;

internal static class EducationPointsCalculator
{
    public static int Calculate(ICollection<Qualification>? qualifications, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Education, IsDeleted: false })
            .ToList();

        if (details.Count == 0 || qualifications == null)
            return 0;

        var maxPoints = details
            .Where(detail => qualifications.Any(q => MatchesEducationDetail(q, detail)))
            .Select(detail => detail.Points)
            .DefaultIfEmpty(0)
            .Max();

        return JobPointsHelpers.Clamp(maxPoints, jobPoints.Education);
    }

    private static bool MatchesEducationDetail(Qualification qualification, JobPointsDetail detail)
    {
        if (detail.ReferenceId.HasValue && qualification.DegreeId == detail.ReferenceId.Value)
            return true;

        return qualification.Degree?.BackendName != null &&
               string.Equals(qualification.Degree.BackendName, detail.Code, StringComparison.OrdinalIgnoreCase);
    }
}
