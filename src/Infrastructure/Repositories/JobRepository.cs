using Application.Operation.Features.Employee.JobManagement.Job.Extensions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobRepository(IGenericRepository<Job> repository)
    : BaseRepository<Job>(repository), IJobRepository
{

    public async Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(
    JobQueryFilter filter,
    PaginatedRequest pagination)
    {
        var baseQuery = Repository.DbSet
            .AsNoTracking()
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.JobTitle)
            .Include(j => j.WorkType)
            .Include(j => j.JobStatus)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation);

        var filteredQuery = baseQuery.ApplyJobFilter(filter);

        var sortedQuery = filteredQuery.ApplySorting(pagination);

        var result = await sortedQuery.ToPaginatedListAsync(pagination);

        return Result.Ok(result);
    }


    public async Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
    {
        var job = await Repository.DbSet
            .AsSplitQuery()
            .Include(j => j.JobDegrees)
            .ThenInclude(d => d.Degree)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.JobTitle)
            .Include(j => j.WorkType)
            .Include(j => j.JobStatus)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation)
            .Include(j => j.JobConditions)
            .Include(j => j.JobPoints)
            .ThenInclude(p => p!.Details)
            .Include(j => j.JobSkills)
            .ThenInclude(s=>s.Skill)
            .Include(j => j.JobResponsibilities)
            .Include(j => j.JobRequiredAttachments)
            .Include(j => j.Invitations)
            .ThenInclude(i => i.History)
            .Include(j => j.TabReviewNotes)
            .Include(j => j.ReviewAttachment)
            .Include(j => j.CandidateFilterSetting)
            .ThenInclude(c => c!.CandidateTypePercentages)
            .Include(j => j.CandidateFilterSetting)
            .ThenInclude(c => c!.NationalityPercentages)
            .Include(j => j.JobSpecializations)
            .ThenInclude(s => s.Major)
            .Include(j => j.JobSpecializations)
            .ThenInclude(s => s.SubMajor)
            .FirstOrDefaultAsync(j => j.Id == id, ct);

        return job is null
            ? Result.Fail<Job>(JobMessages.JobNotFound)
            : Result.Ok(job);
    }
    public async Task<IResult<Job>> GetByIdWithDetailsUnTrackingAsync(Guid id)
    {
        var job = await Repository.DbSet
            .Where(job=> !job.IsDeleted)
            .AsNoTracking()
            .AsSplitQuery()
            .Include(j => j.JobDegrees)
            .ThenInclude(d => d.Degree)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.JobTitle)
            .Include(j => j.WorkType)
            .Include(j => j.JobStatus)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation)
            .Include(j => j.JobConditions)
            .Include(j => j.JobPoints)
                .ThenInclude(p => p!.Details)
            .Include(j => j.JobSkills)
            .ThenInclude(s=>s.Skill)
            .Include(j => j.JobResponsibilities)
            .Include(j => j.JobRequiredAttachments)
            .Include(j => j.Invitations)
                .ThenInclude(i => i.History)
            .Include(j => j.TabReviewNotes)
            .Include(j => j.ReviewAttachment)
            .Include(j => j.CandidateFilterSetting)
                .ThenInclude(c => c!.CandidateTypePercentages)
            .Include(j => j.CandidateFilterSetting)
                .ThenInclude(c => c!.NationalityPercentages)
            .Include(j => j.JobSpecializations)
                .ThenInclude(s => s.Major)
            .Include(j => j.JobSpecializations)
                .ThenInclude(s => s.SubMajor)
            .FirstOrDefaultAsync(j => j.Id == id);

        return job is null
            ? Result.Fail<Job>(JobMessages.JobNotFound)
            : Result.Ok(job);
    }


    public async Task<List<Job>> GetJobsToAutoCloseBatchAsync(DateTimeOffset currentDate, int batchSize)
    {
        var jobs = await Repository.DbSet.AsNoTracking()
            .Where(j => j.JobStatusId != JobStatusIds.Closed 
                        && j.JobStatusId != JobStatusIds.Cancelled
                        && j.JobStatusId != JobStatusIds.Draft
                        && j.JobStatusId != JobStatusIds.NeedUpdate
                        && j.ClosingDate <= currentDate)
            .OrderBy(j => j.ClosingDate)
            .Take(batchSize)
            .ToListAsync();
        return jobs;
    }
    
    public async Task<IList<Job>> GetJobsToAutoCloseAsync(DateTimeOffset currentDate)
    {
         var jobs = await Repository.DbSet
        .AsNoTracking()
        .Where(j => j.ClosingDate <= currentDate && !j.IsDeleted)
        .ToListAsync();

        return jobs;
    }
    
    
    
    public async Task<Job?> LoadJobWithPointsAsync(Guid jobId)
    {
        return await Repository.DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(j => j.JobPoints)
                .ThenInclude(p => p!.Details)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.JobTitle)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.JobDegrees)
            .FirstOrDefaultAsync(j => j.Id == jobId);
    }
}
