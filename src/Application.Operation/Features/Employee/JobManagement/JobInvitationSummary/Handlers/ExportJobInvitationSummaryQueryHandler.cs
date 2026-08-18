using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Handlers;

internal sealed class ExportJobInvitationSummaryQueryHandler(
    JobInvitationSummaryQueryBuilder queryBuilder,
    JobInvitationSummaryExcelExporter excelExporter)
    : IRequestHandler<ExportJobInvitationSummaryQuery,
        IResult<FileExportResult>>
{
    public async Task<IResult<FileExportResult>> Handle(
        ExportJobInvitationSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await queryBuilder
            .ApplySorting(queryBuilder.Build(request), request.SortBy, request.SortDirection)
            .ToListAsync(cancellationToken);
        return Result.Ok(excelExporter.Export(rows));
    }
}
