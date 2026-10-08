using Application.Operation.Features.Employee.Exams.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class ListExamResultReportsQueryValidator
    : AbstractValidator<ListExamResultReportsQuery>
{
    public ListExamResultReportsQueryValidator()
    {
        RuleFor(x => x.CreatedFrom)
            .Must((query, from) =>
                !from.HasValue ||
                !query.CreatedTo.HasValue ||
                from.Value <= query.CreatedTo.Value)
            .WithMessage(
                "CreatedFrom must be less than or equal to CreatedTo.");
    }
}
