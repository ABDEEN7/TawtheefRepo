using Application.Operation.Features.Interview.EvaluationBank.Handlers.Queries;
using Application.Operation.Features.Interview.EvaluationBank.Queries;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationBank.Validators;

public sealed class ListCriteriaByAxisQueryValidator : AbstractValidator<ListCriteriaByAxisQuery>
{
    public ListCriteriaByAxisQueryValidator()
    {
        RuleFor(x => x.InterviewEvaluationAxisId).NotEmpty();
    }
}
