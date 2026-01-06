using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed record AgreeToTermsCommand(Guid UserId) : ICommand<IResult<Unit>>;
