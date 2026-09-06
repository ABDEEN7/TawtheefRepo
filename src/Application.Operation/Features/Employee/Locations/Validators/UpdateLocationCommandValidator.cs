using Application.Operation.Features.Employee.Locations.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Locations.Validators;

public sealed class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();

        RuleFor(x => x.Location.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Location.NameEn)
            .MaximumLength(200);

        RuleFor(x => x.Location.LocationLink)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Location.Notes)
            .MaximumLength(2000);
    }
}
