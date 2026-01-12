using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Validators;

public sealed class UpdateMajorSkillCommandValidator : AbstractValidator<UpdateMajorSkillCommand>
{
    public UpdateMajorSkillCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
