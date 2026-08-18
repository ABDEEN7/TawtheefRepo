using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Extensions;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

internal sealed class ExportJobsQueryHandler(
    IJobRepository jobRepository,
    EmployeeJobAccessContextProvider accessProvider,
    JobsExcelExporter excelExporter)
    : IRequestHandler<ExportJobsQuery, IResult<FileExportResult>>
{
    public async Task<IResult<FileExportResult>> Handle(
        ExportJobsQuery request,
        CancellationToken cancellationToken)
    {
        var access = accessProvider.GetAccess();
        var jobs = await jobRepository
            .GetFilteredJobsQuery(
                request.Filter ?? new JobQueryFilter(),
                access.CurrentUserId,
                access.HasFullAccess)
            .ApplySorting(request.SortBy, request.SortDirection)
            .Select(job => new JobExportRowDto(
                job.JobTitle != null ? job.JobTitle.JobNameAr : string.Empty,
                job.JobTitle != null ? job.JobTitle.JobNameEn : string.Empty,
                job.JobCategory != null ? job.JobCategory.NameAr : string.Empty,
                job.JobCategory != null ? job.JobCategory.NameEn : string.Empty,
                job.Gender != null ? job.Gender.NameAr : string.Empty,
                job.Gender != null ? job.Gender.NameEn : string.Empty,
                job.JobStatus != null ? job.JobStatus.NameAr : string.Empty,
                job.JobStatus != null ? job.JobStatus.NameEn : string.Empty,
                job.Sector != null ? job.Sector.NameAr : string.Empty,
                job.Sector != null ? job.Sector.NameEn : string.Empty,
                job.Management != null ? job.Management.NameAr : string.Empty,
                job.Management != null ? job.Management.NameEn : string.Empty,
                job.Department != null ? job.Department.NameAr : string.Empty,
                job.Department != null ? job.Department.NameEn : string.Empty,
                job.NumberOfVacancies,
                job.ClosingDate,
                job.CreatedBy != null ? job.CreatedBy.FullNameAr : string.Empty,
                job.CreatedBy != null ? job.CreatedBy.FullNameEn : string.Empty,
                job.UpdatedDate ?? job.CreatedDate))
            .ToListAsync(cancellationToken);

        return Result.Ok(excelExporter.Export(jobs));
    }
}
