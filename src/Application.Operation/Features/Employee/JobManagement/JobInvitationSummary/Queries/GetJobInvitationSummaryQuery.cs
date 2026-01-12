using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;

public sealed record GetJobInvitationSummaryQuery(
    Guid? JobCategoryId,
    Guid? DepartmentId,
    Guid? JobStatusId)
    : PaginatedRequest, IQuery<IResult<PaginatedResult<JobInvitationSummaryDto>>>;
