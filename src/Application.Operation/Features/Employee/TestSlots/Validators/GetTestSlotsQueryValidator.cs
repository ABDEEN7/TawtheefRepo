using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.TestSlots.Validators;

public sealed class GetTestSlotsQueryValidator : AbstractValidator<GetTestSlotsQuery>
{
    public GetTestSlotsQueryValidator()
    {
        RuleFor(query => query.DateFrom)
            .Must((query, from) => !from.HasValue || !query.DateTo.HasValue || from.Value <= query.DateTo.Value)
            .WithMessage("DateFrom must not be later than DateTo.");
    }
}
