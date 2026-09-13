using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class UpdateTemplateVersionAxisCommandValidator : AbstractValidator<UpdateTemplateVersionAxisCommand>
{
    public UpdateTemplateVersionAxisCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.QualificationScore).GreaterThanOrEqualTo(0).When(x => x.QualificationScore.HasValue);
        RuleFor(x => x.OrderNo).GreaterThanOrEqualTo(0);
    }
}
