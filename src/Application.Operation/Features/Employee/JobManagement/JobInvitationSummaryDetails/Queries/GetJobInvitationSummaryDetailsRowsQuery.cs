using Application.Operation.Features.Employee.JobInvitationSummaryDetails.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsRowsQuery(
    Guid JobId,
    Guid? StatusId,
    string? Search,
    Guid? BatchNumber)
    : PaginatedRequest, IQuery<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>;
