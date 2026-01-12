using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;

public record DeleteSkillCommand(Guid Id) : ICommand<IResult<Unit>>;
