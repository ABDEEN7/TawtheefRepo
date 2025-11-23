using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record LogoutCommand(Guid UserId) : IRequest<IResult<Unit>>;
