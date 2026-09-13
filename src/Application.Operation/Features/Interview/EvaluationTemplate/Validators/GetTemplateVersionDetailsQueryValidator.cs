using Application.Operation.Features.Interview.EvaluationTemplate.Queries;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class GetTemplateVersionDetailsQueryValidator : AbstractValidator<GetTemplateVersionDetailsQuery>
{
    public GetTemplateVersionDetailsQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
