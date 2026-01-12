using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Validators;

public sealed class CreateMajorSkillCommandValidator : AbstractValidator<CreateMajorSkillCommand>
{
    public CreateMajorSkillCommandValidator()
    {
        RuleFor(x => x.MajorId).NotEmpty();
        RuleFor(x => x.SkillId).NotEmpty();
    }
}
