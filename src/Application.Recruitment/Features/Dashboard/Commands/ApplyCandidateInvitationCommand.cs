using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Commands;

public sealed record ApplyCandidateInvitationCommand(
    Guid UserId,
    Guid InvitationId
) : ICommand<IResult<Unit>>;
