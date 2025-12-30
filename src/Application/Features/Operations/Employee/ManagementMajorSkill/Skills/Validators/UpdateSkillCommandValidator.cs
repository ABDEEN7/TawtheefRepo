using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Validators;

public class UpdateSkillCommandValidator : AbstractValidator<UpdateSkillCommand>
{
    public UpdateSkillCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required.")
            .MaximumLength(200).WithMessage("Arabic name must not exceed 200 characters.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required.")
            .MaximumLength(200).WithMessage("English name must not exceed 200 characters.");

        RuleFor(x => x.BackendName)
            .NotEmpty().WithMessage("BackendName is required.")
            .MaximumLength(50).WithMessage("BackendName must not exceed 50 characters.");

        RuleFor(x => x.SkillTypeId)
            .NotEmpty().WithMessage("SkillTypeId is required.");

        RuleFor(x => x.SkillRequirementTypeId)
            .NotEmpty().WithMessage("SkillRequirementTypeId is required.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}