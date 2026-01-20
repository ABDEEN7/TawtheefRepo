using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public record ConfirmPhoneVerificationCommand(Guid? UserId, string PhoneE164, string Code)
    : ICommand<IResult<Unit>>;
