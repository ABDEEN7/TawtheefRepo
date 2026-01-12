using Application.Operation.Features.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.JobCandidates.Utilities;

internal static class JobCandidateScoringUtility
{

    public static List<JobCandidateRecord> Score(
        List<JobCandidateRecord> window,
        List<UserProfile> profiles,
        Tawtheef.Domain.Entities.Recruitment.Job job,
        JobRequirements req)
    {
        var map = profiles.ToDictionary(p => p.UserId);
        var scored = new List<JobCandidateRecord>(window.Count);

        foreach (var c in window)
        {
            if (!map.TryGetValue(c.ApplicantId, out var p))
                continue;

            var latestQ = p.Qualifications?
                .OrderByDescending(q => q.GraduationYear)
                .FirstOrDefault();

            var latestMajorId = latestQ?.MajorId;
            var latestMajor = latestQ?.Major;

            // Safety re-check
            if (req.JobMajorId.HasValue || req.JobSubMajorId.HasValue)
            {
                var ok = latestMajorId.HasValue &&
                         (latestMajorId.Value == req.JobMajorId || latestMajorId.Value == req.JobSubMajorId);
                if (!ok) continue;
            }

            var candidate = c with
            {
                Applicant = p.User,
                Profile = p,
                Major = latestMajor
            };

            var points = JobCandidatePointsCalculator.Calculate(candidate, job.JobPoints);
            scored.Add(candidate with { Points = points });
        }

        return scored;
    }
}
