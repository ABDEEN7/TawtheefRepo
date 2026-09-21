using Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Validators;

public sealed class ListCriteriaByAxisQueryValidator : AbstractValidator<ListCriteriaByAxisQuery>
{
    public ListCriteriaByAxisQueryValidator()
    {
        RuleFor(x => x.InterviewEvaluationAxisId).NotEmpty();
    }
}
