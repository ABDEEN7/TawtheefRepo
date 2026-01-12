using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsStatsQuery(Guid JobId)
    : IQuery<IResult<JobInvitationSummaryDetailsStatsDto>>;
