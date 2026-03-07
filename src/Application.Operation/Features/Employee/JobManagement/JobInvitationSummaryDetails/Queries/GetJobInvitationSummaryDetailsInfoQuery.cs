using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsInfoQuery(Guid JobId)
    : IRequest<IResult<JobInvitationSummaryDetailsInfoDto>>;

