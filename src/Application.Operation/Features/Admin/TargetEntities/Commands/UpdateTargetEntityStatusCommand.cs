using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Commands;

public sealed record UpdateTargetEntityStatusCommand(Guid TargetEntityId, bool IsActive) : IRequest<IResult<Unit>>;

