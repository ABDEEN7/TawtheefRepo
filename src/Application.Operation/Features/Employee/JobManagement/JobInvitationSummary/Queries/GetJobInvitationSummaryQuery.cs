using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Contracts;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;

public sealed record GetJobInvitationSummaryQuery(
    string? Search,
    Guid? JobCategoryId,
    Guid? DepartmentId,
    Guid? JobStatusId,
    int? Year)
    : PaginatedRequest, IJobInvitationSummaryFilter,
        IRequest<IResult<PaginatedResult<JobInvitationSummaryDto>>>;

