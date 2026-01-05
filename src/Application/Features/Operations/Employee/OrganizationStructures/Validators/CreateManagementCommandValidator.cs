using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Validators;

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
