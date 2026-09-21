using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Validators;

public sealed class ListTemplateVersionsQueryValidator : AbstractValidator<ListTemplateVersionsQuery>
{
    public ListTemplateVersionsQueryValidator()
    {
        RuleFor(x => x.InterviewTemplateId).NotEmpty();
    }
}
