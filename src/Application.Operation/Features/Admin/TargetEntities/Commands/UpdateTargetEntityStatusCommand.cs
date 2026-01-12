using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Commands;

public sealed record UpdateTargetEntityStatusCommand(Guid TargetEntityId, bool IsActive) : ICommand<IResult<Unit>>;
