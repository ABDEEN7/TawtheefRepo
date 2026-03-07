using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Commands;

public sealed record ApplyCandidateInvitationCommand(
    Guid UserId,
    Guid InvitationId
) : IRequest<IResult<Unit>>;

