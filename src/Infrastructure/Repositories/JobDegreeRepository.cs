using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobDegreeRepository(IGenericRepository<JobDegree> repository)
    : BaseRepository<JobDegree>(repository), IJobDegreeRepository
{
    private readonly IGenericRepository<JobDegree> _repository = repository;

    public async Task<IResult<List<JobDegree>>> GetByJobIdAsync(Guid jobId)
    {
        try
        {
            var degrees = await _repository.DbSet
                .Where(jd => jd.JobId == jobId)
                .ToListAsync();
                
            return Result.Ok(degrees);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<JobDegree>>(ex.Message);
        }
    }

    public async Task<IResult<bool>> ExistsForJobAndDegreeAsync(Guid jobId, Guid degreeId)
    {
        try
        {
            var exists = await _repository.DbSet
                .AnyAsync(jd => jd.JobId == jobId && jd.DegreeId == degreeId);
                
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>(ex.Message);
        }
    }

    public async Task<IResult<int>> DeleteByJobIdAsync(Guid jobId)
    {
        try
        {
            var degreesToDelete = await _repository.DbSet
                .Where(jd => jd.JobId == jobId)
                .ToListAsync();

            _repository.DbSet.RemoveRange(degreesToDelete);
            return Result.Ok(degreesToDelete.Count);
        }
        catch (Exception ex)
        {
            return Result.Fail<int>(ex.Message);
        }
    }
}
