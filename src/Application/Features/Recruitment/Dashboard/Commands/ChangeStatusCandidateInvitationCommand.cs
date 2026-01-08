using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Commands;

public sealed record ChangeStatusCandidateInvitationCommand(
    Guid UserId,
    Guid InvitationId,
    string StatusCode
) : ICommand<IResult<Unit>>;
