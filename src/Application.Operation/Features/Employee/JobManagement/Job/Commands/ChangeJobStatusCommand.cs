using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record ChangeJobStatusCommand(Guid JobId,Guid NewStatusId) : ICommand<IResult<Unit>>;
