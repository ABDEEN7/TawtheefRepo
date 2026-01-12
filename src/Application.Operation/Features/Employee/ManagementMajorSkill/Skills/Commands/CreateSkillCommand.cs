using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;

public record CreateSkillCommand(string NameAr, string NameEn, 
    string? DescriptionAr, string? DescriptionEn,
    bool IsActive, Guid SkillTypeId): ICommand<IResult<Unit>>;
