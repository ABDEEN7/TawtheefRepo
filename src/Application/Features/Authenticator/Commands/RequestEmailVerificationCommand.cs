using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RequestEmailVerificationCommand(Guid? UserId, string Email)
    : IRequest<IResult<Unit>>;
