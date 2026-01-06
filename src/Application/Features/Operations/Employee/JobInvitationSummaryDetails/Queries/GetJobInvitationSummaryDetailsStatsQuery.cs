using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;

public sealed record GetJobInvitationSummaryDetailsStatsQuery(Guid JobId)
    : IQuery<IResult<JobInvitationSummaryDetailsStatsDto>>;
