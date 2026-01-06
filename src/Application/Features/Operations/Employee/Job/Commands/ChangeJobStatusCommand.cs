using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record ChangeJobStatusCommand(Guid JobId,Guid NewStatusId) : ICommand<IResult<Unit>>;
