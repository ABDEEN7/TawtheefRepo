using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public record ConfirmEmailVerificationCommand(Guid? UserId, string Email, string Code)
    : ICommand<IResult<Unit>>;
