using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Validators;

public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.ManagementId).NotEmpty();

        RuleFor(x => x.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(200);
    }
}
