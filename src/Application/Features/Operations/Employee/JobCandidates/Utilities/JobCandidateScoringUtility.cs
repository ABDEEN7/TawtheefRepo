using Serilog;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;

internal static class JobCandidateScoringUtility
{

    public static List<JobCandidateRecord> Score(
        List<JobCandidateRecord> window,
        List<UserProfile> profiles,
        JobPointsMain jobPointsMain,
        Guid jobMajorId,
        Guid? jobSubMajorId,
        List<JobDegree> jobDegrees,
        ILogger logger)
    {
        var map = profiles.ToDictionary(p => p.UserId);
        var scored = new List<JobCandidateRecord>(window.Count);

        foreach (var c in window)
        {
            if (!map.TryGetValue(c.ApplicantId, out var p))
                continue;

            var candidate = c with
            {
                Applicant = p.User,
                Profile = p,
            };

            var points = JobCandidatePointsCalculator.Calculate(candidate, jobPointsMain,jobDegrees,jobMajorId,jobSubMajorId,logger);
            scored.Add(candidate with { Points = points });
        }

        return scored;
    }
}
