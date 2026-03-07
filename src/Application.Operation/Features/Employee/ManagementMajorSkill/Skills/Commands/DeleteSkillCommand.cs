using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;

public record DeleteSkillCommand(Guid Id) : IRequest<IResult<Unit>>;

