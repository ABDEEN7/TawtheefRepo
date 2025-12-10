using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Commands;

public sealed record UpdateUserRolesCommand(Guid UserId, IReadOnlyCollection<Guid> RoleIds)
    : IRequest<IResult>;
