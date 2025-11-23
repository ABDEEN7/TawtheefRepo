using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ConfirmEmailVerificationCommand(Guid? UserId, string Email, string Code)
    : IRequest<IResult<Unit>>;
