using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public record RequestEmailVerificationCommand(Guid? UserId, string Email)
    : ICommand<IResult<Unit>>;
