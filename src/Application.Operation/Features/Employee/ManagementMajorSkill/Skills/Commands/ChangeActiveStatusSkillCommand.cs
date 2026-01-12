using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;

public record ChangeActiveStatusSkillCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = false) : ICommand<IResult<Unit>>;
