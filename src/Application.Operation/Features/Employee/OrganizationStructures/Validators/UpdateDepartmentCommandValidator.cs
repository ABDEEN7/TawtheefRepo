using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.OrganizationStructures.Validators;

public sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ManagementId).NotEmpty();

        RuleFor(x => x.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(200);
    }
}
