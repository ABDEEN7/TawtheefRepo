using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsRowsQuery(
    Guid JobId,
    Guid? StatusId,
    string? Search,
    Guid? BatchNumber)
    : PaginatedRequest, IQuery<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>;
