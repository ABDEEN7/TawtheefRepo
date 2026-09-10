using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class UpdateTemplateVersionCommandValidator : AbstractValidator<UpdateTemplateVersionCommand>
{
    public UpdateTemplateVersionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FinalScore).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QualificationScore).GreaterThanOrEqualTo(0).When(x => x.QualificationScore.HasValue);
        RuleFor(x => x.CalculationMethod).IsInEnum();
    }
}
