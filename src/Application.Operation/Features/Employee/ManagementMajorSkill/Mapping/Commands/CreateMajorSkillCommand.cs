using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;

public record CreateMajorSkillCommand(
    Guid MajorId,
    Guid SkillId,
    bool IsSkillRequired,
    bool IsActive
) : ICommand<IResult<Unit>>;
