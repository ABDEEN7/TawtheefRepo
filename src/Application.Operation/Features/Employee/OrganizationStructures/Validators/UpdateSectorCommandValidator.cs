using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.OrganizationStructures.Validators;

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
