using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;

internal static class SkillPointsCalculator
{
    public static int Calculate(ICollection<ProfileSkill>? skills, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Skill, IsDeleted: false })
            .ToList();

        if (details.Count == 0 || skills == null)
            return 0;

        var skillIds = skills
            .Select(s => s.SkillId)
            .ToHashSet();

        var points = details
            .Where(detail =>
                (detail.ReferenceId.HasValue && skillIds.Contains(detail.ReferenceId.Value)) ||
                skills.Any(skill =>
                    skill.Skill?.BackendName != null &&
                    string.Equals(skill.Skill.BackendName, detail.Code, StringComparison.OrdinalIgnoreCase)))
            .Sum(detail => detail.Points);

        return JobPointsHelpers.Clamp(points, jobPoints.Skills);
    }
}
