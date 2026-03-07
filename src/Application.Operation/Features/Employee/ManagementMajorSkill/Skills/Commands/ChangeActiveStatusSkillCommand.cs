using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;

public record ChangeActiveStatusSkillCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = false) : IRequest<IResult<Unit>>;

