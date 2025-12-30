using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Validators;

public sealed class UpdateMajorSkillCommandValidator : AbstractValidator<UpdateMajorSkillCommand>
{
    public UpdateMajorSkillCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
