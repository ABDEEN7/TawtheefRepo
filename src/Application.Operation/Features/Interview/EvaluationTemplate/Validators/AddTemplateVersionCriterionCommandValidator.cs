using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class AddTemplateVersionCriterionCommandValidator : AbstractValidator<AddTemplateVersionCriterionCommand>
{
    public AddTemplateVersionCriterionCommandValidator()
    {
        RuleFor(x => x.InterviewTemplateEvaluationAxisId).NotEmpty();
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.OrderNo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(1000);

        // A criterion is either picked from the evaluation bank (no name override) or fully custom (needs NameAr).
        When(x => x.InterviewEvaluationCriterionId.HasValue, () =>
        {
            RuleFor(x => x.NameAr).Empty();
            RuleFor(x => x.NameEn).Empty();
            RuleFor(x => x.DescriptionAr).Empty();
            RuleFor(x => x.DescriptionEn).Empty();
        }).Otherwise(() =>
        {
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NameEn).MaximumLength(200);
        });
    }
}
