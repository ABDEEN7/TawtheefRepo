using Application.Operation.Features.Employee.CandidateUsers.Queries;
using Application.Operation.Features.Employee.CandidateUsers.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Queries;

internal sealed class ExportCandidateUsersQueryHandler(
    CandidateUsersQueryBuilder queryBuilder,
    CandidateUsersExcelExporter excelExporter)
    : IRequestHandler<ExportCandidateUsersQuery, IResult<FileExportResult>>
{
    public async Task<IResult<FileExportResult>> Handle(
        ExportCandidateUsersQuery request,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBuilder.BuildAsync(request, cancellationToken);
        if (queryResult.IsFailed)
            return Result.Fail<FileExportResult>(queryResult.Errors);

        var users = await queryResult.Value
            .OrderBy(user => user.FullNameEn)
            .ThenBy(user => user.Id)
            .ToListAsync(cancellationToken);

        return Result.Ok(excelExporter.Export(users));
    }
}
