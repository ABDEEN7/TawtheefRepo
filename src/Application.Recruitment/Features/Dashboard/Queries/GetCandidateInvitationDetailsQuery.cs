using Application.Recruitment.Features.Dashboard.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Queries;

public sealed record GetCandidateInvitationDetailsQuery(
    Guid InvitationId,
    Guid UserId
) : IRequest<IResult<CandidateInvitationsDto>>;

