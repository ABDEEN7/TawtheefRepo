using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Validators;

public sealed class UpdateSectorCommandValidator : AbstractValidator<UpdateSectorCommand>
{
    public UpdateSectorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(200);
    }
}
