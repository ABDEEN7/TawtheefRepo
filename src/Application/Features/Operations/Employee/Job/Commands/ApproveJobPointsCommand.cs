using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record ApproveJobPointsCommand(Guid JobId) : ICommand<IResult<bool>>;
