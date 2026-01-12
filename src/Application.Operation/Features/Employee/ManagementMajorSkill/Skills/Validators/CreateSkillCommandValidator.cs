using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Validators;


public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required.")
            .MaximumLength(200).WithMessage("Arabic name must not exceed 200 characters.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required.")
            .MaximumLength(200).WithMessage("English name must not exceed 200 characters.");

        RuleFor(x => x.SkillTypeId)
            .NotEmpty().WithMessage("SkillTypeId is required.");
    }
}
