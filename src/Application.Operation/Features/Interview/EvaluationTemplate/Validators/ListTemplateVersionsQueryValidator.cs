using Application.Operation.Features.Interview.EvaluationTemplate.Queries;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class ListTemplateVersionsQueryValidator : AbstractValidator<ListTemplateVersionsQuery>
{
    public ListTemplateVersionsQueryValidator()
    {
        RuleFor(x => x.InterviewTemplateId).NotEmpty();
    }
}
