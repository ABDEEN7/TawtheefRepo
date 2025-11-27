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

public class JobRepository (IGenericRepository<Job> repository) : BaseRepository<Job>(repository), IJobRepository
{
    private readonly IGenericRepository<Job> _repository = repository;

    public async Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(
        JobQueryFilter filter,
        PaginatedRequest pagination)
    {
        var query = _repository.DbSet.AsQueryable();

        query = query
            .Include(j => j.RequestingDepartment)
            .Include(j => j.JobCategory)
            .Include(j => j.WorkType)
            .Include(j => j.Status)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.WorkLocation)
            .Include(j => j.Skills)
            .Include(j => j.Conditions)
            .Include(j => j.Degrees).ThenInclude(
                d => d.Degree)
            .Include(j => j.Invitations)
            .Include(j => j.Quota)
            .ThenInclude(q => q!.ResidentsBreakdowns);
        

        query = query.ApplyJobFilter(filter);

        var result = await query.ToPaginatedListAsync(pagination);

        return Result.Ok(result);
    }
    
    public async Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id)
    {
        var job = await _repository.DbSet
            .Include(j => j.RequestingDepartment)
            .Include(j => j.JobCategory)
            .Include(j => j.WorkType)
            .Include(j => j.Status)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.WorkLocation)
            .Include(j => j.Skills)
            .Include(j => j.Conditions)
            .Include(j => j.Degrees)
            .ThenInclude(de => de.Degree)
            .Include(j => j.Invitations)
            .Include(j => j.Quota)
            .ThenInclude(q => q!.ResidentsBreakdowns).ThenInclude(rs=>rs.Nationality)
            .FirstOrDefaultAsync(j => j.Id == id);

        return job is null ? Result.Fail<Job>(JobValidationMessages.JobNotFound) : Result.Ok(job);
    }
}
