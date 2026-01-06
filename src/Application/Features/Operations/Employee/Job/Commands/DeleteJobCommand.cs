using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record DeleteJobCommand(Guid JobId) : ICommand<IResult<Unit>>;
