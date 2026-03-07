using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;

public record CreateMajorSkillCommand(
    Guid MajorId,
    Guid SkillId,
    bool IsSkillRequired,
    bool IsActive
) : IRequest<IResult<Unit>>;

