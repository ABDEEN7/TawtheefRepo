using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record BlockOfficeUserCommand(Guid OfficeId, Guid UserId, bool IsBlocked)
    : ICommand<IResult<Unit>>;
