using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.CandidateUsers.Commands;

public sealed record UpdateCandidateUserBlockStatusCommand : ICommand<IResult<Unit>>
{
    public Guid UserId { get; init; }
    public bool IsBlocked { get; init; }
}
