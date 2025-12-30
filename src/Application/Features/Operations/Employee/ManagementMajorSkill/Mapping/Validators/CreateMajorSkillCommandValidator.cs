using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Validators;

public sealed class CreateMajorSkillCommandValidator : AbstractValidator<CreateMajorSkillCommand>
{
    public CreateMajorSkillCommandValidator()
    {
        RuleFor(x => x.MajorId).NotEmpty();
        RuleFor(x => x.SkillId).NotEmpty();

        RuleFor(x => x.Weight)
            .InclusiveBetween(MajorSkillRules.WeightMin, MajorSkillRules.WeightMax)
            .WithMessage($"Weight must be between {MajorSkillRules.WeightMin} and {MajorSkillRules.WeightMax}.");
    }
}