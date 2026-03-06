using Application.Recruitment.Features.JobDetails.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.JobDetails.Queries;

public sealed record GetCandidateJobDetailsQuery(
    Guid InvitationId,
    Guid UserId
) : IRequest<IResult<CandidateJobDetailsDto>>;

