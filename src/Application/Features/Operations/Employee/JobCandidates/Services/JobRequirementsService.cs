using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;

internal static class JobRequirementsService
{
    public static async Task<JobRequirements> GetAsync(
        IUnitOfWork unitOfWork,
        Domain.Entities.Recruitment.Job job,
        CancellationToken ct)
    {
        var jobMajorId = job.MajorId;
        var jobSubMajorId = job.SubMajorId ?? job.SubMajor?.Id;

        var majorIds = new List<Guid>();
        majorIds.Add(jobMajorId);
        if (jobSubMajorId.HasValue) majorIds.Add(jobSubMajorId.Value);

        List<Guid> requiredSkillIds = [];

        if (majorIds.Count > 0)
        {
            requiredSkillIds = await unitOfWork.GetEntityRepository<MajorSkill>().DbSet
                .AsNoTracking()
                .Where(ms => majorIds.Contains(ms.MajorId) && ms.IsActive && ms.IsSkillRequired)
                .Select(ms => ms.SkillId)
                .Distinct()
                .ToListAsync(ct);
        }

        return new JobRequirements(jobMajorId, jobSubMajorId, requiredSkillIds);
    }
}
