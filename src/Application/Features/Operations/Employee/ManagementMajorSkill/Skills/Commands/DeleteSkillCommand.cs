using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;

public record DeleteSkillCommand(Guid Id) : IRequest<IResult<Unit>>;