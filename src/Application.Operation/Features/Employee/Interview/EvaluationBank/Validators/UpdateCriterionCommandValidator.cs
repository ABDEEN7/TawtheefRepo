using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Validators;

public sealed class UpdateCriterionCommandValidator : AbstractValidator<UpdateCriterionCommand>
{
    public UpdateCriterionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.InterviewEvaluationAxisId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).MaximumLength(200);
    }
}
