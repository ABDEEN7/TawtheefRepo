using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public class JobRequirementsService(IUnitOfWork unitOfWork) : IJobRequirementsService
{
    public async Task<JobRequirements> GetAsync(Tawtheef.Domain.Entities.Recruitment.Job job)
    {
        var majorIds = new List<Guid>();
        if (job.MajorId.HasValue && job.MajorId.Value != Guid.Empty)
            majorIds.Add(job.MajorId.Value);

        if (job.SubMajorId.HasValue && job.SubMajorId.Value != Guid.Empty)
            majorIds.Add(job.SubMajorId.Value);
        
        var jobSpecializations = job.JobSpecializations?.Select(js =>
                new JobSpecialization(js.MajorId, js.SubMajorId))
            .ToList() ?? [];
        jobSpecializations.ForEach(js =>
        {
            if (js.MajorId != Guid.Empty && !majorIds.Contains(js.MajorId))
                majorIds.Add(js.MajorId);
            if (js.SubMajorId != Guid.Empty && !majorIds.Contains(js.SubMajorId))
                majorIds.Add(js.SubMajorId);
        });

        List<Guid> requiredSkillIds = [];

        var qualificationLevelIds = job.JobDegrees
            .Select(jd => jd.DegreeId).ToList();
        
        if (majorIds.Count > 0)
        {
            requiredSkillIds = await unitOfWork.GetEntityRepository<MajorSkill>().DbSet
                .AsNoTracking()
                .Where(ms => majorIds.Contains(ms.MajorId) && ms.IsActive && ms.IsSkillRequired)
                .Select(ms => ms.SkillId)
                .Distinct()
                .ToListAsync();
        }

        return new JobRequirements(job.MajorId, job.SubMajorId, 
            jobSpecializations, qualificationLevelIds, requiredSkillIds);
    }
}
