using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using jobEntity = Tawtheef.Domain.Entities.Recruitment.Job;
namespace Tawtheef.Application.Features.Operations.Employee.Job.Extensions;

public static class JobRepositoryExtensions
{
    public static IQueryable<jobEntity> ApplySorting(this IQueryable<jobEntity> query, PaginatedRequest? pagination)
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
    public static IQueryable<jobEntity> ApplyJobFilter(
        this IQueryable<jobEntity> query,
        JobQueryFilter? filter)
    {
        if (filter is null)
            return query;

        return query

            // Search term
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SearchTerm),
                j => j.Title.Contains(filter.SearchTerm!) ||
                     j.Description.Contains(filter.SearchTerm!) ||
                     j.Benefits.Contains(filter.SearchTerm!))

            // Department
            .WhereIf(filter.DepartmentId.HasValue,
                j => j.RequestingDepartmentId == filter.DepartmentId)

            // Status
            .WhereIf(filter.StatusId.HasValue,
                j => j.StatusId == filter.StatusId)

            // Category
            .WhereIf(filter.JobCategoryId.HasValue,
                j => j.JobCategoryId == filter.JobCategoryId)

            // Work type
            .WhereIf(filter.WorkTypeId.HasValue,
                j => j.WorkTypeId == filter.WorkTypeId)

            // Deadline range
            .WhereIf(filter.DeadlineFrom.HasValue,
                j => j.Deadline >= filter.DeadlineFrom)

            .WhereIf(filter.DeadlineTo.HasValue,
                j => j.Deadline <= filter.DeadlineTo)

            // Vacancies min/max
            .WhereIf(filter.MinVacancies.HasValue,
                j => j.Vacancies >= filter.MinVacancies)

            .WhereIf(filter.MaxVacancies.HasValue,
                j => j.Vacancies <= filter.MaxVacancies);
    }
}
