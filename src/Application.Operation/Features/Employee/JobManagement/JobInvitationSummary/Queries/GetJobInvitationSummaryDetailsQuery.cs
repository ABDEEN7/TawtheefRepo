using Application.Operation.Features.Employee.JobInvitationSummary.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobInvitationSummary.Queries;

public sealed record GetJobInvitationSummaryDetailsQuery( Guid? JobId) : PaginatedRequest, IQuery<IResult<PaginatedResult<JobInvitationSummaryDto>>>;
