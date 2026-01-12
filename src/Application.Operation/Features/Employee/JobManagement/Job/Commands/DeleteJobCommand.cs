using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record DeleteJobCommand(Guid JobId) : ICommand<IResult<Unit>>;
