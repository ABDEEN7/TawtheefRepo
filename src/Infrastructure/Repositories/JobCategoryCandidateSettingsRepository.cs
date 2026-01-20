using Application.Operation.Common.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobCategoryCandidateSettingsRepository(IGenericRepository<JobCategoryCandidateSettings> repository)
    : BaseRepository<JobCategoryCandidateSettings>(repository), IJobCategoryCandidateSettingsRepository
{
    public async Task<IResult<JobCategoryCandidateSettings>> GetAsync()
    {
        var settings = await Repository.DbSet.FirstOrDefaultAsync();

        return settings == null
            ? Result.Fail<JobCategoryCandidateSettings>(JobCandidatesMessages.JobCategoryCandidateSettingsNotFound)
            : Result.Ok(settings);
    }
}
