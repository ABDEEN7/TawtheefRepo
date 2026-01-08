using Cortex.Mediator;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Recruitment.JobDetails.DTOs;

namespace Tawtheef.Application.Features.Recruitment.JobDetails.Queries;

public sealed record GetCandidateJobDetailsQuery(
    Guid InvitationId,
    Guid UserId
) : IQuery<IResult<CandidateJobDetailsDto>>;
