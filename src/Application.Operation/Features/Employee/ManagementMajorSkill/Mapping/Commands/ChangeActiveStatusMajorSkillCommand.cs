using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;

public record ChangeActiveStatusMajorSkillCommand(Guid Id, bool IsActive)
    : ICommand<IResult<Unit>>;
