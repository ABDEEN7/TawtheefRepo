using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public record ConfirmPhoneVerificationCommand(Guid? UserId, string PhoneE164, string Code)
    : ICommand<IResult<Unit>>;
