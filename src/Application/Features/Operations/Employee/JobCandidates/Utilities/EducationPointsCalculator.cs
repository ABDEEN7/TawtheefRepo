using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;

internal static class EducationPointsCalculator
{
    public static int Calculate(
        ICollection<Qualification>? qualifications,
        JobPointsMain jobPoints,
        List<JobDegree> jobDegrees,
        Guid? jobMajorId,
        Guid? jobSubMajorId)
    {
        if (qualifications == null || qualifications.Count == 0)
            return 0;

        if (jobDegrees.Count == 0)
            return 0;

        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Education, IsDeleted: false })
            .ToList();

        if (details.Count == 0)
            return 0;

        var allowedDegreeIds = jobDegrees
            .Select(d => d.DegreeId)
            .ToHashSet();

        var hasMajorReq = jobMajorId.HasValue || jobSubMajorId.HasValue;

        var validQualifications = qualifications
            .Where(q =>
            {
                if (!hasMajorReq) return allowedDegreeIds.Contains(q.DegreeId);
                var majorOk =
                    (q.MajorId.HasValue &&
                     (q.MajorId.Value == jobMajorId || q.MajorId.Value == jobSubMajorId))
                    ||
                    (q.SubMajorId.HasValue &&
                     (q.SubMajorId.Value == jobMajorId || q.SubMajorId.Value == jobSubMajorId));

                return majorOk &&
                       allowedDegreeIds.Contains(q.DegreeId);
            })
            .ToList();

        if (hasMajorReq && validQualifications.Count == 0)
            return 0;

        var totalPoints = validQualifications
            .Select(q => details
                .Where(detail => MatchesEducationDetail(q, detail))
                .Select(detail => detail.Points)
                .DefaultIfEmpty(0)
                .Max())
            .Sum();

        return JobPointsHelpers.Clamp(totalPoints, jobPoints.Education);
    }

    private static bool MatchesEducationDetail(Qualification qualification, JobPointsDetail detail)
    {
        if (detail.ReferenceId.HasValue && qualification.DegreeId == detail.ReferenceId.Value)
            return true;

        return qualification.Degree?.BackendName != null &&
               string.Equals(qualification.Degree.BackendName, detail.Code, StringComparison.OrdinalIgnoreCase);
    }
}
