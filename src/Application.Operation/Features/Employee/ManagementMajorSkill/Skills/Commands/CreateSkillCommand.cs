using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;

public record CreateSkillCommand(string NameAr, string NameEn, 
    string? DescriptionAr, string? DescriptionEn,
    bool IsActive, Guid SkillTypeId): IRequest<IResult<Unit>>;

