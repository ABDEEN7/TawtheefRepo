using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;

public record UpdateMajorSkillCommand(
    Guid Id,
    bool IsSkillRequired,
    bool IsActive
) : ICommand<IResult<Unit>>;
