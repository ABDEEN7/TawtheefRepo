using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RequestEmailVerificationCommand(Guid? UserId, string Email)
    : ICommand<IResult<Unit>>;
