using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
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
            .Include(j => j.Quota)
                .ThenInclude(q => q!.ResidentsBreakdowns)
                    .ThenInclude(rb => rb.Nationality)
            .Include(j => j.Degrees)
                .ThenInclude(d => d.Degree)
            .Include(j => j.RequestingDepartment)
            .Include(j => j.JobCategory)
            .Include(j => j.WorkType)
            .Include(j => j.Status)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation)
            .Include(j => j.Conditions)
            .Include(j => j.Skills)
            .Include(j => j.Responsibilities)
            .Include(j => j.RequiredAttachments)
            .Include(j => j.Invitations);

        var filteredQuery = baseQuery.ApplyJobFilter(filter);

        var sortedQuery = filteredQuery.ApplySorting(pagination);

        var result = await sortedQuery.ToPaginatedListAsync(pagination);

        return Result.Ok(result);
    }


    public async Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id)
    {
        var job = await _repository.DbSet
            .AsNoTracking()
            .Include(j => j.Quota)
                .ThenInclude(q => q!.ResidentsBreakdowns)
                    .ThenInclude(rb => rb.Nationality)
            .Include(j => j.Degrees)
                .ThenInclude(d => d.Degree)
            .Include(j => j.RequestingDepartment)
            .Include(j => j.JobCategory)
            .Include(j => j.WorkType)
            .Include(j => j.Status)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation)
            .Include(j => j.Conditions)
            .Include(j => j.Skills)
            .Include(j => j.Responsibilities)
            .Include(j => j.RequiredAttachments)
            .Include(j => j.Invitations)

            .FirstOrDefaultAsync(j => j.Id == id);

        return job is null
            ? Result.Fail<Job>(JobValidationMessages.JOB_NOT_FOUND)
            : Result.Ok(job);
    }
}
