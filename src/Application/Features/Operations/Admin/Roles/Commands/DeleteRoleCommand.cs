using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Commands;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<IResult<Unit>>;
