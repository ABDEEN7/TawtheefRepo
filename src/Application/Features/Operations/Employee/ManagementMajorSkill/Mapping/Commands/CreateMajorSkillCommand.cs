using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;

public record CreateMajorSkillCommand(
    Guid MajorId,
    Guid SkillId,
    bool IsSkillRequired,
    bool IsActive
) : IRequest<IResult<Unit>>;
