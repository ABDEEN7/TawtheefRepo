using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;

/// <summary>
/// Utility class responsible for calculating the education points for a job candidate based on their qualifications and the job's point rules and requirements.
/// </summary>
internal static class EducationPointsCalculator
{
    /// <summary>
    /// Calculates the education points for a candidate based on their qualifications and the job's education point rules and requirements.
    /// </summary>
    /// <param name="qualifications">The candidate's qualifications.</param>
    /// <param name="jobPoints">The job's education point rules and requirements.</param>
    /// <param name="jobDegrees">The degrees accepted for the job.</param>
    /// <param name="jobMajorId">The required major ID for the job, if any.</param>
    /// <param name="jobSubMajorId">The required sub-major ID for the job, if any.</param>
    /// <returns>The calculated education points for the candidate.</returns>
    public static int Calculate(
        ICollection<Qualification>? qualifications,
        JobPointsMain jobPoints,
        ICollection<JobDegree> jobDegrees,
        Guid? jobMajorId,
        Guid? jobSubMajorId)
    {
        // If the candidate has no qualifications, they cannot get education points.
        if (qualifications == null || qualifications.Count == 0)
            return 0;

        // If the job has no accepted degrees configured, no qualification can match.
        if (jobDegrees.Count == 0)
            return 0;

        // Get only active education point rules from the job point details.
        var details = jobPoints.Details
            // Keep details where:
            // - Type is Education
            // - IsDeleted is false
            .Where(d => d is { Type: JobPointRuleType.Education, IsDeleted: false })

            // Convert the filtered rules to a list so they can be reused.
            .ToList();

        // If there are no active education rules, no education points can be awarded.
        if (details.Count == 0)
            return 0;

        // Build a fast lookup set of degree IDs allowed by the job.
        var allowedDegreeIds = jobDegrees
            // Extract the DegreeId from each job degree.
            .Select(d => d.DegreeId)
            // Convert to HashSet for faster Contains checks.
            .ToHashSet();

        // Determine whether the job requires a specific major or sub-major.
        var hasMajorReq = jobMajorId.HasValue || jobSubMajorId.HasValue;

        // Filter the candidate qualifications to only those valid for this job.
        var validQualifications = qualifications
            .Where(q =>
            {
                // If the job does not require a major or sub-major,
                // the qualification only needs to have an allowed degree.
                if (!hasMajorReq) return allowedDegreeIds.Contains(q.DegreeId);

                // Check whether the qualification's major or sub-major matches
                // the job's required major or required sub-major.
                var majorOk =
                    // Then check the qualification's SubMajorId.
                    (q.SubMajorId.HasValue &&
                     // SubMajorId matches either the job major or job sub-major.
                     (q.SubMajorId.Value == jobMajorId || q.SubMajorId.Value == jobSubMajorId));

                // Qualification is valid only if:
                // - major/sub-major matches
                // - degree is allowed for the job
                return majorOk &&
                       allowedDegreeIds.Contains(q.DegreeId);
            })

            // Store valid qualifications in a list for later processing.
            .ToList();

        // If the job requires a major/sub-major but none of the candidate's
        // qualifications match that requirement, return 0.
        if (hasMajorReq && validQualifications.Count == 0)
            return 0;

        // Calculate the total points from all valid qualifications.
        var totalPoints = validQualifications
            // For each valid qualification, find the best matching education rule.
            .Select(q => details
                // Keep only rules that match this qualification.
                .Where(detail => MatchesEducationDetail(q, detail))

                // Select the points from each matching rule.
                .Select(detail => detail.Points)

                // If no rule matches, use 0 points.
                .DefaultIfEmpty(0)

                // Take the highest matching points for this qualification.
                .Max())

            // Add the best points from each valid qualification.
            .Sum();

        // Return the calculated total points, but do not allow it to exceed
        // the maximum education points configured in jobPoints.Education.
        return JobPointsHelpers.Clamp(totalPoints, jobPoints.Education);
    }

    // Checks whether a qualification matches a specific education point rule.
    private static bool MatchesEducationDetail(Qualification qualification, JobPointsDetail detail)
    {
        // If the rule has a ReferenceId and it equals the qualification's DegreeId,
        // then the rule matches this qualification.
        if (detail.ReferenceId.HasValue && qualification.DegreeId == detail.ReferenceId.Value)
            return true;

        // Otherwise, match by degree BackendName and rule Code.
        return qualification.Degree?.BackendName != null &&

               // Compare the qualification degree BackendName with the detail Code.
               // OrdinalIgnoreCase means the comparison is case-insensitive.
               string.Equals(
                   qualification.Degree.BackendName,
                   detail.Code,
                   StringComparison.OrdinalIgnoreCase);
    }
}
