using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobConditionRepository(IGenericRepository<JobCondition> repository)
    : BaseRepository<JobCondition>(repository), IJobConditionRepository
{
    private readonly IGenericRepository<JobCondition> _repository = repository;

    public async Task<IResult<List<JobCondition>>> GetByJobIdAsync(Guid jobId)
    {
        try
        {
            var conditions = await _repository.DbSet
                .Where(jc => jc.JobId == jobId)
                .ToListAsync();
                
            return Result.Ok(conditions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<JobCondition>>(ex.Message);
        }
    }

    public async Task<IResult<List<JobCondition>>> GetByJobIdOrderedAsync(Guid jobId)
    {
        try
        {
            var conditions = await _repository.DbSet
                .Where(jc => jc.JobId == jobId)
                .OrderBy(jc => jc.Order)
                .ToListAsync();
                
            return Result.Ok(conditions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<JobCondition>>(ex.Message);
        }
    }

    public async Task<IResult<int>> DeleteByJobIdAsync(Guid jobId)
    {
        try
        {
            var conditionsToDelete = await _repository.DbSet
                .Where(jc => jc.JobId == jobId)
                .ToListAsync();

            _repository.DbSet.RemoveRange(conditionsToDelete);
            return Result.Ok(conditionsToDelete.Count);
        }
        catch (Exception ex)
        {
            return Result.Fail<int>(ex.Message);
        }
    }
}
