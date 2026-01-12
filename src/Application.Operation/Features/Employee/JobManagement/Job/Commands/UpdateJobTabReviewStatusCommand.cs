using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public sealed record UpdateJobTabReviewStatusCommand(
    Guid JobId
) : ICommand<IResult<Unit>>;
