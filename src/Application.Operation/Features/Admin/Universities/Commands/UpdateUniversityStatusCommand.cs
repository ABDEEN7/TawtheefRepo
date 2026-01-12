using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Universities.Commands;

public sealed record UpdateUniversityStatusCommand(Guid UniversityId, bool IsActive) : ICommand<IResult<Unit>>;
