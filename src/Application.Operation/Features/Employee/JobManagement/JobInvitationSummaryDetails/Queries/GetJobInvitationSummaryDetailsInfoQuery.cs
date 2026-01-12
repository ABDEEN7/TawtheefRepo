using Application.Operation.Features.Employee.JobInvitationSummaryDetails.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsInfoQuery(Guid JobId)
    : IQuery<IResult<JobInvitationSummaryDetailsInfoDto>>;
