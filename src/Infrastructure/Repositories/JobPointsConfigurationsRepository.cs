using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobPointsConfigurationsRepository(IGenericRepository<JobPointConfiguration> repository)
    : BaseRepository<JobPointConfiguration>(repository), IJobPointsConfigurationsRepository
{

    public async Task<IResult<JobPointConfiguration>> GetByJobIdAsync(Guid jobId)
    {
        var jobPointsConfiguration = await Repository.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.JobId == jobId);

        return jobPointsConfiguration == null
            ? Result.Fail<JobPointConfiguration>(JobMessages.JOB_POINTS_NOT_FOUND)
            : Result.Ok(jobPointsConfiguration);
    }
}
