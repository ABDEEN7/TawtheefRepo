using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Roles.Commands;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<IResult<Unit>>;

