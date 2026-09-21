using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Validators;

public sealed class UpdateTemplateVersionCriterionCommandValidator : AbstractValidator<UpdateTemplateVersionCriterionCommand>
{
    public UpdateTemplateVersionCriterionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.OrderNo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(1000);

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
