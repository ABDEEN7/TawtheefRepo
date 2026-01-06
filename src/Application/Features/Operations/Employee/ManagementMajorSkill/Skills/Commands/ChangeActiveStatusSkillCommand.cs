using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;

public record ChangeActiveStatusSkillCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = false) : ICommand<IResult<Unit>>;
