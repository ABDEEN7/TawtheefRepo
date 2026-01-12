using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Commands;
public sealed record ChangeStatusCandidateInvitationRejectedCommand(
    Guid UserId,
    Guid InvitationId
) : ICommand<IResult<Unit>>;
