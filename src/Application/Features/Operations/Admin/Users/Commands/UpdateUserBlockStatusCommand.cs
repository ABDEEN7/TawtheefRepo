using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Commands;

public sealed record UpdateUserBlockStatusCommand(Guid UserId, bool IsBlocked) : IRequest<IResult>;
