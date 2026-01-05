using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Commands;

public sealed record UpdateReligionStatusCommand(Guid ReligionId, bool IsActive) : ICommand<IResult<Unit>>;
