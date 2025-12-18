using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobRepository(IGenericRepository<Job> repository)
    : BaseRepository<Job>(repository), IJobRepository
{
    private readonly IGenericRepository<Job> _repository = repository;

    public async Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(
    JobQueryFilter filter,
    PaginatedRequest pagination)
    {
        var baseQuery = _repository.DbSet
            .AsNoTracking()
            .Where(job => !job.IsDeleted)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
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


    public async Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id)
    {
        var job = await _repository.DbSet
            .Where(job=> !job.IsDeleted)
            .Include(j => j.JobDegrees)
                .ThenInclude(d => d.Degree)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.WorkType)
            .Include(j => j.JobStatus)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation)
            .Include(j => j.JobConditions)
            .Include(j => j.JobSkills)
                   .ThenInclude(s=>s.Skill)
            .Include(j => j.JobResponsibilities)
            .Include(j => j.JobRequiredAttachments)
            .Include(j => j.Invitations)
            .FirstOrDefaultAsync(j => j.Id == id);

        return job is null
            ? Result.Fail<Job>(JobMessages.JOB_NOT_FOUND)
            : Result.Ok(job);
    }

    public async Task<IList<Job>> GetJobsToAutoCloseAsync(DateTime currentDate)
    {
         var jobs = await _repository.DbSet
        .AsNoTracking()
        .Where(j => j.ClosingDate <= currentDate && !j.IsDeleted)
        .ToListAsync();

        return jobs;
    }
}
