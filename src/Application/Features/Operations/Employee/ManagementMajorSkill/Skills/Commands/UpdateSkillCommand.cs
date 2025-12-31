using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;

public record UpdateSkillCommand(Guid Id, string NameAr, string NameEn, 
    string? DescriptionAr, string? DescriptionEn,
    bool IsActive, Guid SkillTypeId): IRequest<IResult<Unit>>;
