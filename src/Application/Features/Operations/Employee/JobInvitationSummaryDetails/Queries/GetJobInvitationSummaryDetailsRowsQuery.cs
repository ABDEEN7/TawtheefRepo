using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsRowsQuery(
    Guid JobId,
    Guid? StatusId,
    string? Search)
    : PaginatedRequest, IRequest<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>;
