using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public record RequestUpdatePhoneCommand(Guid? UserId, string PhoneE164)
    : ICommand<IResult<Unit>>;
