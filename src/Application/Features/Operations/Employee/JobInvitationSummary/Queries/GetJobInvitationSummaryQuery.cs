using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.Queries;

public sealed record GetJobInvitationSummaryQuery(
    Guid? JobCategoryId,
    Guid? DepartmentId,
    Guid? JobStatusId)
    : PaginatedRequest, IRequest<IResult<PaginatedResult<JobInvitationSummaryDto>>>;
