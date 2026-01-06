using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public sealed record UpdateJobTabReviewStatusCommand(
    Guid JobId
) : ICommand<IResult<Unit>>;
