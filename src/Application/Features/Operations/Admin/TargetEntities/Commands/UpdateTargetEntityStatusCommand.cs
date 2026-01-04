using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Commands;

public sealed record UpdateTargetEntityStatusCommand(Guid TargetEntityId, bool IsActive) : IRequest<IResult<Unit>>;
