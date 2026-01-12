using Application.Operation.Common.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobTabReviewNoteRepository(IGenericRepository<JobTabReviewNote> repository)
    : BaseRepository<JobTabReviewNote>(repository), IJobTabReviewNoteRepository
{
    public async Task<IResult<List<JobTabReviewNote>>> GetByIdWithDetailsAsync(Guid jobId)
    {
        var result = await Repository.DbSet
            .AsNoTracking()
            .Where(t => t.JobId == jobId)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        return Result.Ok(result);
    }

    public async Task<IResult<List<JobTabReviewNote>>> GetLastReviewCycleAsync(Guid jobId)
    {
        var lastCycleId = await Repository.DbSet
            .Where(x => x.JobId == jobId)
            .GroupBy(x => x.ReviewCycleId)
            .OrderByDescending(g => g.Max(x => x.CreatedDate))
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        if (lastCycleId == Guid.Empty)
            return Result.Ok(new List<JobTabReviewNote>());

        var result = await Repository.DbSet
            .AsNoTracking()
            .Where(x => x.JobId == jobId && x.ReviewCycleId == lastCycleId)
            .ToListAsync();

        return Result.Ok(result);
    }
}
