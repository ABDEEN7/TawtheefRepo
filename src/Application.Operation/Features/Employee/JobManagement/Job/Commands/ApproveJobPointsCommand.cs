using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public record ApproveJobPointsCommand(Guid JobId) : ICommand<IResult<bool>>;
