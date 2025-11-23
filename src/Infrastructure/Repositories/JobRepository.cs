using System.Linq.Expressions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobRepository(IGenericRepository<Job> repository) : BaseRepository<Job>(repository), IJobRepository
{
    public async Task<IResult<Job>> GetByIdAsync(Guid id, params Expression<Func<Job, object>>[]? includes)
    {
        var query = Repository.DbSet.AsQueryable();

        if (includes is null || includes.Length <= 0)
        {
            var job = await query.FirstOrDefaultAsync(j => j.Id == id);
            return job != null ? Result.Ok(job) : Result.Fail<Job>("Job not found");
        }

        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var result = await query.FirstOrDefaultAsync(j => j.Id == id);
        
        return result != null ? Result.Ok(result) : Result.Fail<Job>("Job not found");
    }

    public async Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id)
    {
        var job = await Repository.DbSet
            .Include(j => j.Quotas)
                .ThenInclude(q => q.ResidentsBreakdown)
                    .ThenInclude(rb => rb.Nationality)
            .Include(j => j.Skills)
            .Include(j => j.Conditions)
            .Include(j => j.Degrees)
                .ThenInclude(d => d.Degree)
            .Include(j => j.Status)
            .Include(j => j.RequestingDepartment)
            .Include(j => j.Major)
            .Include(j => j.WorkType)
            .Include(j => j.Gender)
            .Include(j => j.JobCategory)
            .Include(j => j.TargetEntity)
            .FirstOrDefaultAsync(j => j.Id == id);
            
        return job != null ? Result.Ok(job) : Result.Fail<Job>("Job not found");
    }

    public async Task<IResult<PaginatedResult<Job>>> GetPaginatedJobsAsync(PaginatedRequest request)
    {
        try
        {
            var result = await Repository.DbSet
                .Include(j => j.Status)
                .Include(j => j.RequestingDepartment)
                .Include(j => j.JobCategory)
                .ToPaginatedListAsync(request);
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<PaginatedResult<Job>>($"Failed to get paginated jobs: {ex.Message}");
        }
    }

    public async Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(JobQueryFilter filter, PaginatedRequest pagination)
    {
        try
        {
            var query =  Repository.DbSet
                .Include(j => j.Status)
                .Include(j => j.RequestingDepartment)
                .Include(j => j.JobCategory)
                .AsQueryable();

            // Apply filters
            query = ApplyFilters(query, filter);

            // Apply sorting
            bool sortDescending = pagination.SortDirection?.ToLower() == "desc";
            query = ApplySorting(query, pagination.SortBy, sortDescending);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToListAsync();

            return Result.Ok(new PaginatedResult<Job>(items, totalCount, pagination.PageNumber, pagination.PageSize));
        }
        catch (Exception ex)
        {
            return Result.Fail<PaginatedResult<Job>>($"Failed to get filtered jobs: {ex.Message}");
        }
    }

    private static IQueryable<Job> ApplyFilters(IQueryable<Job> query, JobQueryFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(j => j.Title.Contains(filter.SearchTerm) ||
                                   (j.Description != null && j.Description.Contains(filter.SearchTerm)));
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(j => j.RequestingDepartmentId == filter.DepartmentId.Value);
        }

        if (filter.StatusId.HasValue)
        {
            query = query.Where(j => j.StatusId == filter.StatusId.Value);
        }

        if (filter.JobCategoryId.HasValue)
        {
            query = query.Where(j => j.JobCategoryId == filter.JobCategoryId.Value);
        }

        if (filter.WorkTypeId.HasValue)
        {
            query = query.Where(j => j.WorkTypeId == filter.WorkTypeId.Value);
        }

        if (filter.DeadlineFrom.HasValue)
        {
            query = query.Where(j => j.Deadline >= filter.DeadlineFrom.Value);
        }

        if (filter.DeadlineTo.HasValue)
        {
            query = query.Where(j => j.Deadline <= filter.DeadlineTo.Value);
        }

        if (filter.MinVacancies.HasValue)
        {
            query = query.Where(j => j.Vacancies >= filter.MinVacancies.Value);
        }

        if (filter.MaxVacancies.HasValue)
        {
            query = query.Where(j => j.Vacancies <= filter.MaxVacancies.Value);
        }

        return query;
    }

    private static IQueryable<Job> ApplySorting(IQueryable<Job> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLower() switch
        {
            "title" => sortDescending 
                ? query.OrderByDescending(j => j.Title)
                : query.OrderBy(j => j.Title),
            "deadline" => sortDescending 
                ? query.OrderByDescending(j => j.Deadline)
                : query.OrderBy(j => j.Deadline),
            "vacancies" => sortDescending 
                ? query.OrderByDescending(j => j.Vacancies)
                : query.OrderBy(j => j.Vacancies),
            "created" => sortDescending 
                ? query.OrderByDescending(j => j.CreatedDate)
                : query.OrderBy(j => j.CreatedDate),
            _ => sortDescending 
                ? query.OrderByDescending(j => j.CreatedDate)
                : query.OrderBy(j => j.CreatedDate)
        };
    }
}
