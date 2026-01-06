using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record LogoutCommand(Guid UserId) : ICommand<IResult<Unit>>;
