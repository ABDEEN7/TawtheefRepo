using Application.Operation.Features.Interview.EvaluationBank.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationBank.Validators;

public sealed class CreateCriterionCommandValidator : AbstractValidator<CreateCriterionCommand>
{
    public CreateCriterionCommandValidator()
    {
        RuleFor(x => x.InterviewEvaluationAxisId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).MaximumLength(200);
    }
}
