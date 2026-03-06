using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsRowsQuery(
    Guid JobId,
    Guid? StatusId,
    string? Search,
    Guid? BatchNumber)
    : PaginatedRequest, IRequest<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>;

