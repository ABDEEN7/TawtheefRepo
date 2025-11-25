using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobSkillRepository(IGenericRepository<JobSkill> repository)
    : BaseRepository<JobSkill>(repository), IJobSkillRepository
{
    private readonly IGenericRepository<JobSkill> _repository = repository;

    public async Task<IResult<List<JobSkill>>> GetByJobIdAsync(Guid jobId)
    {
        try
        {
            var skills = await _repository.DbSet
                .Where(js => js.JobId == jobId)
                .ToListAsync();
                
            return Result.Ok(skills);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<JobSkill>>(ex.Message);
        }
    }

    public async Task<IResult<List<JobSkill>>> GetByJobIdOrderedAsync(Guid jobId)
    {
        try
        {
            var skills = await _repository.DbSet
                .Where(js => js.JobId == jobId)
                .OrderBy(js => js.Order)
                .ToListAsync();
                
            return Result.Ok(skills);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<JobSkill>>(ex.Message);
        }
    }

    public async Task<IResult<int>> DeleteByJobIdAsync(Guid jobId)
    {
        try
        {
            var skillsToDelete = await _repository.DbSet
                .Where(js => js.JobId == jobId)
                .ToListAsync();

            _repository.DbSet.RemoveRange(skillsToDelete);
            return Result.Ok(skillsToDelete.Count);
        }
        catch (Exception ex)
        {
            return Result.Fail<int>(ex.Message);
        }
    }
}
