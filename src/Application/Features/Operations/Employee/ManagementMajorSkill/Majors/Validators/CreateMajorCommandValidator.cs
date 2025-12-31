using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Validators;

public class CreateMajorCommandValidator : AbstractValidator<CreateMajorCommand>
{
    public CreateMajorCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required.")
            .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required.")
            .MaximumLength(100).WithMessage("English name must not exceed 100 characters.");
    }
}