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
            return query.OrderByDescending(j => j.CreatedDate)
                        .ThenBy(j => j.Title);
        }

        var sort = pagination.SortBy.ToLower();
        var desc = pagination.SortDirection?.ToLower() == "desc";

        return sort switch
        {
            "title" => desc ? query.OrderByDescending(j => j.Title) : query.OrderBy(j => j.Title),
            "deadline" => desc ? query.OrderByDescending(j => j.Deadline) : query.OrderBy(j => j.Deadline),
            "vacancies" => desc ? query.OrderByDescending(j => j.Vacancies) : query.OrderBy(j => j.Vacancies),
            "created" or "createddate"
                           => desc ? query.OrderByDescending(j => j.CreatedDate) : query.OrderBy(j => j.CreatedDate),
            "updated" or "updateddate"
                           => desc ? query.OrderByDescending(j => j.UpdatedDate) : query.OrderBy(j => j.UpdatedDate),

            _ => query.OrderByDescending(j => j.CreatedDate)
        };
    }

    public static IQueryable<jobEntity> ApplyJobFilter(
    this IQueryable<jobEntity> query,
    JobQueryFilter? filter)
    {
        if (filter is null)
            return query;

        return query

            // SEARCH
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SearchTerm),
                j => j.Title.Contains(filter.SearchTerm!) ||
                     j.Description.Contains(filter.SearchTerm!) ||
                     j.Benefits.Contains(filter.SearchTerm!) ||
                     (j.Overview ?? "").Contains(filter.SearchTerm!) ||
                     (j.QualificationsDescription ?? "").Contains(filter.SearchTerm!)
            )

            // BASIC LOOKUP FILTERS
            .WhereIf(filter.StatusId.HasValue, j => j.StatusId == filter.StatusId)
            .WhereIf(filter.DepartmentId.HasValue, j => j.RequestingDepartmentId == filter.DepartmentId)
            .WhereIf(filter.JobCategoryId.HasValue, j => j.JobCategoryId == filter.JobCategoryId)
            .WhereIf(filter.WorkTypeId.HasValue, j => j.WorkTypeId == filter.WorkTypeId)
            .WhereIf(filter.SectorId.HasValue, j => j.SectorId == filter.SectorId)
            .WhereIf(filter.ManagementId.HasValue, j => j.ManagementId == filter.ManagementId)
            .WhereIf(filter.WorkLocationId.HasValue, j => j.WorkLocationId == filter.WorkLocationId)
            .WhereIf(filter.GenderId.HasValue, j => j.GenderId == filter.GenderId)
            .WhereIf(filter.MajorId.HasValue, j => j.MajorId == filter.MajorId)
            .WhereIf(filter.SubMajorId.HasValue, j => j.SubMajorId == filter.SubMajorId)

            // AGE RANGE
            .WhereIf(filter.MinAge.HasValue, j => j.MinimumAge >= filter.MinAge)
            .WhereIf(filter.MaxAge.HasValue, j => j.MaximumAge <= filter.MaxAge)

            // EXPERIENCE
            .WhereIf(filter.MinExperienceYears.HasValue, j => j.MinimumExperienceYears >= filter.MinExperienceYears)

            // VACANCIES
            .WhereIf(filter.MinVacancies.HasValue, j => j.Vacancies >= filter.MinVacancies)
            .WhereIf(filter.MaxVacancies.HasValue, j => j.Vacancies <= filter.MaxVacancies)

            // DEADLINE
            .WhereIf(filter.DeadlineFrom.HasValue, j => j.Deadline >= filter.DeadlineFrom)
            .WhereIf(filter.DeadlineTo.HasValue, j => j.Deadline <= filter.DeadlineTo)

            // PUBLISH DATE
            .WhereIf(filter.PublishFrom.HasValue, j => j.PublishAt >= filter.PublishFrom)
            .WhereIf(filter.PublishTo.HasValue, j => j.PublishAt <= filter.PublishTo);
    }
}
