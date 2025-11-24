using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Extensions;

public static class JobRepositoryExtensions
{
    public static IQueryable<Domain.Entities.Recruitment.Job> ApplyIncludes(this IQueryable<Domain.Entities.Recruitment.Job> query, List<Expression<Func<Domain.Entities.Recruitment.Job, object>>>? includes)
    {
        if (includes == null || !includes.Any()) 
            return query;

        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public static IQueryable<Domain.Entities.Recruitment.Job> ApplySorting(this IQueryable<Domain.Entities.Recruitment.Job> query, PaginatedRequest? pagination)
    {
        if (pagination == null || string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            // Default sorting
            return query.OrderByDescending(j => j.CreatedDate)
                .ThenBy(j => j.Title);
        }

        var isDescending = pagination.SortDirection?.ToLower() == "desc";
        
        return pagination.SortBy.ToLower() switch
        {
            "title" => isDescending ? 
                query.OrderByDescending(j => j.Title) : 
                query.OrderBy(j => j.Title),
            "deadline" => isDescending ? 
                query.OrderByDescending(j => j.Deadline) : 
                query.OrderBy(j => j.Deadline),
            "vacancies" => isDescending ? 
                query.OrderByDescending(j => j.Vacancies) : 
                query.OrderBy(j => j.Vacancies),
            "createdDate" or "created" => isDescending ? 
                query.OrderByDescending(j => j.CreatedDate) : 
                query.OrderBy(j => j.CreatedDate),
            "updatedDate" or "updated" => isDescending ? 
                query.OrderByDescending(j => j.UpdatedDate) : 
                query.OrderBy(j => j.UpdatedDate),
            _ => query.OrderByDescending(j => j.CreatedDate)
        };
    }

    // Your existing ApplyJobFilter method remains the same
    public static IQueryable<Domain.Entities.Recruitment.Job> ApplyJobFilter(this IQueryable<Domain.Entities.Recruitment.Job> query, JobQueryFilter? filter)
    {
        if (filter == null) return query;

        // Search term filter
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(j => 
                j.Title.Contains(filter.SearchTerm) ||
                (j.Description != null && j.Description.Contains(filter.SearchTerm)) ||
                (j.Benefits != null && j.Benefits.Contains(filter.SearchTerm)));
        }

        // Department filter
        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(j => j.RequestingDepartmentId == filter.DepartmentId.Value);
        }

        // Status filter
        if (filter.StatusId.HasValue)
        {
            query = query.Where(j => j.StatusId == filter.StatusId.Value);
        }

        // Job category filter
        if (filter.JobCategoryId.HasValue)
        {
            query = query.Where(j => j.JobCategoryId == filter.JobCategoryId.Value);
        }

        // Work type filter
        if (filter.WorkTypeId.HasValue)
        {
            query = query.Where(j => j.WorkTypeId == filter.WorkTypeId.Value);
        }

        // Deadline range filter
        if (filter.DeadlineFrom.HasValue)
        {
            query = query.Where(j => j.Deadline >= filter.DeadlineFrom.Value);
        }

        if (filter.DeadlineTo.HasValue)
        {
            query = query.Where(j => j.Deadline <= filter.DeadlineTo.Value);
        }

        // Vacancies range filter
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
}
