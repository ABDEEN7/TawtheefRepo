using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Commands;

public sealed record UpdateUniversityStatusCommand(Guid UniversityId, bool IsActive) : ICommand<IResult<Unit>>;
