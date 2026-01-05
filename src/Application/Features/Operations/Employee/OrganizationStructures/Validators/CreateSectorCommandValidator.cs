using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Validators;

public sealed class CreateSectorCommandValidator : AbstractValidator<CreateSectorCommand>
{
    public CreateSectorCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(200);
    }
}
