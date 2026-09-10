using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class AddTemplateVersionAxisCommandValidator : AbstractValidator<AddTemplateVersionAxisCommand>
{
    public AddTemplateVersionAxisCommandValidator()
    {
        RuleFor(x => x.InterviewTemplateVersionId).NotEmpty();
        RuleFor(x => x.InterviewEvaluationAxisId).NotEmpty();
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.QualificationScore).GreaterThanOrEqualTo(0).When(x => x.QualificationScore.HasValue);
        RuleFor(x => x.OrderNo).GreaterThanOrEqualTo(0);
    }
}
