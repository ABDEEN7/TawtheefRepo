using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Commands;

public sealed record UpdateUserBlockStatusCommand(Guid UserId, bool IsBlocked) : IRequest<IResult<Unit>>;

