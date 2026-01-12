using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public record ChangeJobStatusCommand(Guid JobId,Guid NewStatusId) : ICommand<IResult<Unit>>;
