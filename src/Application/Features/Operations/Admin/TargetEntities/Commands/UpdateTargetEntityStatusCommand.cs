using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Commands;

public sealed record UpdateTargetEntityStatusCommand(Guid TargetEntityId, bool IsActive) : ICommand<IResult<Unit>>;
