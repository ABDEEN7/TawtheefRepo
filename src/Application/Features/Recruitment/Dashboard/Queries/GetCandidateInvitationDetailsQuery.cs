using Cortex.Mediator;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationDetailsQuery(
    Guid InvitationId,
    Guid UserId
) : IQuery<IResult<CandidateInvitationsDto>>;
