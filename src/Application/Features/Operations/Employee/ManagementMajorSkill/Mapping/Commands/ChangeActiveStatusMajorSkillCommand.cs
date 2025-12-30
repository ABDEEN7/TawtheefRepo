using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;

public record ChangeActiveStatusMajorSkillCommand(Guid Id, bool IsActive)
    : IRequest<IResult<Unit>>;