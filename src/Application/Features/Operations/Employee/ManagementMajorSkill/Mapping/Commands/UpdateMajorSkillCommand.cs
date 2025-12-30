using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;

public record UpdateMajorSkillCommand(
    Guid Id,
    bool IsSkillRequired,
    bool IsActive
) : IRequest<IResult<Unit>>;
