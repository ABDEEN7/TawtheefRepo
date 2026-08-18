using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Export;

using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Contracts;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;

public sealed record ExportJobInvitationSummaryQuery(
    string? Search,
    Guid? JobCategoryId,
    Guid? DepartmentId,
    Guid? JobStatusId,
    int? Year,
    string? SortBy,
    string? SortDirection)
    : IJobInvitationSummaryFilter, IRequest<IResult<FileExportResult>>;
