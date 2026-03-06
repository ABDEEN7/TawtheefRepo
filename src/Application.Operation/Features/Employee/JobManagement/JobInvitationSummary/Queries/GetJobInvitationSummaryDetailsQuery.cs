using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;

public sealed record GetJobInvitationSummaryDetailsQuery( Guid? JobId) : PaginatedRequest, IRequest<IResult<PaginatedResult<JobInvitationSummaryDto>>>;

