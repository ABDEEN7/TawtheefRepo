using Application.Operation.Features.Employee.Exams.Queries;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class ListExamsQueryValidator : AbstractValidator<ListExamsQuery>
{
    public ListExamsQueryValidator()
    {
        RuleFor(query => query.CreatedFrom)
            .Must((query, from) => !from.HasValue || !query.CreatedTo.HasValue ||
                                   from.Value <= query.CreatedTo.Value)
            .WithErrorCode(ErrorsCodes.ExamCreatedDateRangeInvalid)
            .WithMessage(ErrorsCodes.ExamCreatedDateRangeInvalid);
    }
}
