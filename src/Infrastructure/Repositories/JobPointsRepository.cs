using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobPointsRepository(IGenericRepository<JobPointsMain> repository)
    : BaseRepository<JobPointsMain>(repository), IJobPointsRepository
{
    private readonly IGenericRepository<JobPointsMain> _repository = repository;

    public async Task<IResult<JobPointsMain>> GetByJobIdAsync(Guid jobId)
    {
        var jobPoints = await _repository.DbSet
            .AsNoTracking()
            .Include(p => p.Details) 
            .FirstOrDefaultAsync(p => p.JobId == jobId);

        if (jobPoints == null)
            return Result.Fail<JobPointsMain>($"Job points for JobId '{jobId}' not found.");

        return Result.Ok(jobPoints);
    }
}
