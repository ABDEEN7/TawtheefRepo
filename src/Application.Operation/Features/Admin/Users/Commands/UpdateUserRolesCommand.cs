using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Commands;

public sealed record UpdateUserRolesCommand(Guid UserId, IReadOnlyCollection<Guid> RoleIds)
    : IRequest<IResult<Unit>>;

