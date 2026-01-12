using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record DeleteMajorCommand(Guid Id): ICommand<IResult<Unit>>;
