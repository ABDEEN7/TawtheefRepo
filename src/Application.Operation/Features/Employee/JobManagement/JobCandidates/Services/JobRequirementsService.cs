using Application.Operation.Features.Employee.JobCandidates.Models;
using Application.Operation.Features.Employee.JobCandidates.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.JobCandidates.Services;

public class JobRequirementsService(IUnitOfWork unitOfWork) : IJobRequirementsService
{
    public async Task<JobRequirements> GetAsync(Guid mainMajorId,Guid? subMajorId)
    {
        var majorIds = new List<Guid>()
        {
            mainMajorId
        };
        
        if(subMajorId != null)
            majorIds.Add(subMajorId.Value);
        

        List<Guid> requiredSkillIds = [];

        if (majorIds.Count > 0)
        {
            requiredSkillIds = await unitOfWork.GetEntityRepository<MajorSkill>().DbSet
                .AsNoTracking()
                .Where(ms => majorIds.Contains(ms.MajorId) && ms.IsActive && ms.IsSkillRequired)
                .Select(ms => ms.SkillId)
                .Distinct()
                .ToListAsync();
        }

        return new JobRequirements(mainMajorId, subMajorId, requiredSkillIds);
    }
}
