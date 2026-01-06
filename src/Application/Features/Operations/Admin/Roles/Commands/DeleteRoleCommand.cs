using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Commands;

public sealed record DeleteRoleCommand(Guid Id) : ICommand<IResult<Unit>>;
