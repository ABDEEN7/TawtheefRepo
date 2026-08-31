using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Queries;

public sealed class GetExceptionJobsQueryHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider,
    ILocalizationService localizationService,
    TimeProvider timeProvider)
    : IRequestHandler<GetExceptionJobsQuery, IResult<PaginatedResult<ExceptionJobLookupDto>>>
{
    public async Task<IResult<PaginatedResult<ExceptionJobLookupDto>>> Handle(
        GetExceptionJobsQuery request,
        CancellationToken cancellationToken)
    {
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var searchTerm = request.SearchTerm?.Trim();
        var isArabic = string.Equals(
            localizationService.GetCurrentLanguage(),
            "ar",
            StringComparison.OrdinalIgnoreCase);

        var jobs = unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess())
            .Where(job =>
                job.ManagementId == request.ManagementId &&
                (!request.DepartmentId.HasValue || job.DepartmentId == request.DepartmentId) &&
                job.JobStatusId == JobStatusIds.Published &&
                job.ClosingDate > utcNow);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            jobs = jobs.Where(job =>
                job.JobTitle != null &&
                (job.JobTitle.JobNameAr.Contains(searchTerm) ||
                 job.JobTitle.JobNameEn.Contains(searchTerm) ||
                 job.JobTitle.JobNumber.Contains(searchTerm)));
        }

        var totalCount = await jobs.CountAsync(cancellationToken);
        var items = await jobs
            .OrderBy(job => job.ClosingDate)
            .ThenBy(job => job.JobTitle!.JobNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(job => new ExceptionJobLookupDto(
                job.Id,
                job.JobTitle != null ? job.JobTitle.JobNumber : string.Empty,
                job.JobTitle != null
                    ? (isArabic ? job.JobTitle.JobNameAr : job.JobTitle.JobNameEn)
                    : string.Empty,
                job.ClosingDate,
                job.JobStatusId,
                job.JobStatus != null
                    ? (isArabic ? job.JobStatus.NameAr : job.JobStatus.NameEn)
                    : string.Empty))
            .ToListAsync(cancellationToken);

        return Result.Ok(new PaginatedResult<ExceptionJobLookupDto>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize));
    }
}
