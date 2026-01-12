using Application.Recruitment.Features.JobDetails.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.JobDetails.Queries;

public sealed record GetCandidateJobDetailsQuery(
    Guid InvitationId,
    Guid UserId
) : IQuery<IResult<CandidateJobDetailsDto>>;
