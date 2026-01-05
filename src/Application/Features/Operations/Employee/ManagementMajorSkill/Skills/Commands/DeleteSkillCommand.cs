using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;

public record DeleteSkillCommand(Guid Id) : ICommand<IResult<Unit>>;
