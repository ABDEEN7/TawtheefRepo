using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Roles.Commands;

public sealed record DeleteRoleCommand(Guid Id) : ICommand<IResult<Unit>>;
