using Application.Operation.Features.Employee.JobInvitationSummaryDetails.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsStatsQuery(Guid JobId)
    : IQuery<IResult<JobInvitationSummaryDetailsStatsDto>>;
