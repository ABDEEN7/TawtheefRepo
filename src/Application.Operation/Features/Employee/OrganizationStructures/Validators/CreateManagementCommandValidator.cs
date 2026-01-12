using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.OrganizationStructures.Validators;

public sealed class CreateManagementCommandValidator : AbstractValidator<CreateManagementCommand>
{
    public CreateManagementCommandValidator()
    {
        RuleFor(x => x.SectorId).NotEmpty();

        RuleFor(x => x.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(200);
    }
}
