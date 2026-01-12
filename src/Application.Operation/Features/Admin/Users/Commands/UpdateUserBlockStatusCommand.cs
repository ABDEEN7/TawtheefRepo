using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Commands;

public sealed record UpdateUserBlockStatusCommand(Guid UserId, bool IsBlocked) : ICommand<IResult<Unit>>;
