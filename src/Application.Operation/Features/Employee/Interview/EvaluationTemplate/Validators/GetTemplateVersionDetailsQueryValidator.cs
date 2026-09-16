using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Validators;

public sealed class GetTemplateVersionDetailsQueryValidator : AbstractValidator<GetTemplateVersionDetailsQuery>
{
    public GetTemplateVersionDetailsQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
