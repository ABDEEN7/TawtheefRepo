using System.Linq.Expressions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
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
        PaginatedRequest pagination,
        List<Expression<Func<Job, object>>> includes)
    {

            // Start with the base query
            var query = _repository.DbSet.AsQueryable();

            // Apply includes (eager loading)
            query = query.ApplyIncludes(includes);

            // Apply filters
            query = query.ApplyJobFilter(filter);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = query.ApplySorting(pagination);

            // Apply pagination if provided
            query = query.Skip(pagination.Skip).Take(pagination.Take);

            // Execute query and get results
            var items = await query.ToListAsync();

            // Create paginated result
            var result = new PaginatedResult<Job>(
                items,
                totalCount,
                pagination?.PageNumber ?? 1,
                pagination?.PageSize ?? totalCount
            );

            return Result.Ok(result);
        
    }
    
    public async Task<IResult<Job?>> GetByIdWithDetailsAsync(Guid id)
    {
            var job = await _repository.DbSet
                .Include(j => j.RequestingDepartment)
                .Include(j => j.JobCategory)
                .Include(j => j.WorkType)
                .Include(j => j.Status)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
                return Result.Fail<Job?>($"{JobValidationMessages.JobNotFound}");
            return Result.Ok(job);
    }
}
