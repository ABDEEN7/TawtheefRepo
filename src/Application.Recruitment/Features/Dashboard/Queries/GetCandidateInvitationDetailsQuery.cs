using Application.Recruitment.Features.Dashboard.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Queries;

public sealed record GetCandidateInvitationDetailsQuery(
    Guid InvitationId,
    Guid UserId
) : IQuery<IResult<CandidateInvitationsDto>>;
