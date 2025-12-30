using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;

public record ChangeActiveStatusSkillCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = false) : IRequest<IResult<Unit>>;
