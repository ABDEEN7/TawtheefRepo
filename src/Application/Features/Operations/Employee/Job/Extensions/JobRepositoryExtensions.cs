using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using jobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Extensions;

public static class JobRepositoryExtensions
{
    extension(IQueryable<jobEntity> query)
    {
        public IQueryable<jobEntity> ApplySorting(PaginatedRequest? pagination)
        {
            if (pagination == null || string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                return query.OrderByDescending(j => j.CreatedDate)
                    .ThenBy(j => j.TitleEn);
            }

            var sort = pagination.SortBy.ToLower();
            var desc = pagination.SortDirection?.ToLower() == "desc";

            return sort switch
            {
                "title" => desc ? query.OrderByDescending(j => j.TitleEn) : query.OrderBy(j => j.TitleEn),
                "deadline" => desc ? query.OrderByDescending(j => j.ClosingDate) : query.OrderBy(j => j.ClosingDate),
                "vacancies" => desc ? query.OrderByDescending(j => j.NumberOfVacancies) : query.OrderBy(j => j.NumberOfVacancies),
                "created" or "createddate"
                    => desc ? query.OrderByDescending(j => j.CreatedDate) : query.OrderBy(j => j.CreatedDate),
                "updated" or "updateddate"
                    => desc ? query.OrderByDescending(j => j.UpdatedDate) : query.OrderBy(j => j.UpdatedDate),

                _ => query.OrderByDescending(j => j.CreatedDate)
            };
        }

        public IQueryable<jobEntity> ApplyJobFilter(JobQueryFilter? filter)
        {
            if (filter is null)
                return query;

            return query

                // SEARCH
                .WhereIf(!string.IsNullOrWhiteSpace(filter.SearchTerm),
                    j => j.TitleAr.Contains(filter.SearchTerm!) ||
                         j.TitleEn.Contains(filter.SearchTerm!) ||
                         j.BenefitsAr!.Contains(filter.SearchTerm!) ||
                         j.BenefitsEn!.Contains(filter.SearchTerm!) ||
                         (j.OverViewAr ?? "").Contains(filter.SearchTerm!) ||
                         (j.OverViewEn ?? "").Contains(filter.SearchTerm!)
                )

                // BASIC LOOKUP FILTERS
                .WhereIf(filter.StatusId.HasValue, j => j.JobStatusId == filter.StatusId)
                .WhereIf(filter.DepartmentId.HasValue, j => j.DepartmentId == filter.DepartmentId)
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
                .WhereIf(filter.MinExperienceYears.HasValue, j => j.YearsOfExperience >= filter.MinExperienceYears)

                // VACANCIES
                .WhereIf(filter.MinVacancies.HasValue, j => j.NumberOfVacancies >= filter.MinVacancies)
                .WhereIf(filter.MaxVacancies.HasValue, j => j.NumberOfVacancies <= filter.MaxVacancies)

                // DEADLINE
                .WhereIf(filter.CloseDateFrom.HasValue, j => j.ClosingDate >= filter.CloseDateFrom)
                .WhereIf(filter.CloseDateTo.HasValue, j => j.ClosingDate <= filter.CloseDateTo);
        }
    }
}
