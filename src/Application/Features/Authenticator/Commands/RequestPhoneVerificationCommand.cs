using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RequestPhoneVerificationCommand(Guid? UserId, string PhoneE164)
    : ICommand<IResult<Unit>>;
