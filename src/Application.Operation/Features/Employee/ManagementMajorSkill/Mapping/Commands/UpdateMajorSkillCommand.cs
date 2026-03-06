using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;

public record UpdateMajorSkillCommand(
    Guid Id,
    bool IsSkillRequired,
    bool IsActive
) : IRequest<IResult<Unit>>;

