using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;

public record ChangeActiveStatusMajorSkillCommand(Guid Id, bool IsActive)
    : IRequest<IResult<Unit>>;

