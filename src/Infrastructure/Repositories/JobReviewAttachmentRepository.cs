using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;
public class JobReviewAttachmentRepository(IGenericRepository<JobReviewAttachment> repository)
    : BaseRepository<JobReviewAttachment>(repository), IJobReviewAttachmentRepository
{
    public async Task<IResult<List<JobReviewAttachment>>> GetByIdWithDetailsAsync(Guid jobId)
    {
        var result = await Repository.DbSet
            .AsNoTracking()
            .Where(t => t.JobId == jobId)
            .Include(att=>att.Attachment)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        return Result.Ok(result);
    }

    public async Task<IResult<List<JobReviewAttachment>>> GetLastReviewCycleAsync(Guid jobId)
    {
        var lastCycleId = await Repository.DbSet
            .Where(x => x.JobId == jobId)
            .GroupBy(x => x.ReviewCycleId)
            .OrderByDescending(g => g.Max(x => x.CreatedDate))
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        if (lastCycleId == Guid.Empty)
            return Result.Ok(new List<JobReviewAttachment>());

        var result = await Repository.DbSet
            .AsNoTracking()
            .Where(x => x.JobId == jobId && x.ReviewCycleId == lastCycleId)
            .Include(att => att.Attachment)
            .ToListAsync();

        return Result.Ok(result);
    }
}
