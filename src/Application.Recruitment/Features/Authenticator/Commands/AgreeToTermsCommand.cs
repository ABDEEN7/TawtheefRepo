using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public sealed record AgreeToTermsCommand(Guid UserId) : ICommand<IResult<Unit>>;
