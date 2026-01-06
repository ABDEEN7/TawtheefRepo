using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Commands;

public sealed record UpdateUserBlockStatusCommand(Guid UserId, bool IsBlocked) : ICommand<IResult<Unit>>;
