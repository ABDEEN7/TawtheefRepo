using Application.Operation.Common.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobPointsRepository(IGenericRepository<JobPointsMain> repository, IAppLogger logger)
    : BaseRepository<JobPointsMain>(repository), IJobPointsRepository
{
    

    public async Task<IResult<JobPointsMain>> GetByJobIdAsync(Guid jobId)
    {
        var jobPoints = await Repository.DbSet
            .Include(p => p.Details) 
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p => p.JobId == jobId);

        if (jobPoints == null)
        {
            logger.Error($"Job points for JobId '{jobId}' not found.");
            return Result.Fail<JobPointsMain>(JobMessages.JobPointsNotFound);
        }

        return Result.Ok(jobPoints);
    }
}
