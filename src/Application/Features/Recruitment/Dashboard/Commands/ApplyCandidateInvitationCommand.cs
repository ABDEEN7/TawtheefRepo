using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Commands;

public sealed record ApplyCandidateInvitationCommand(
    Guid UserId,
    Guid InvitationId
) : ICommand<IResult<Unit>>;
