using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public record DeleteJobCommand(Guid JobId) : ICommand<IResult<Unit>>;
