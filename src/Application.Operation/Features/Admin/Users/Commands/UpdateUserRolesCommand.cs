using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Commands;

public sealed record UpdateUserRolesCommand(Guid UserId, IReadOnlyCollection<Guid> RoleIds)
    : ICommand<IResult<Unit>>;
