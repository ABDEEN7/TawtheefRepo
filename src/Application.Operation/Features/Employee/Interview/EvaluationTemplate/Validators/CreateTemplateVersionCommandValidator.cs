using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Validators;

public sealed class CreateTemplateVersionCommandValidator : AbstractValidator<CreateTemplateVersionCommand>
{
    public CreateTemplateVersionCommandValidator()
    {
        RuleFor(x => x.InterviewTemplateId).NotEmpty();
        RuleFor(x => x.FinalScore).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QualificationScore).GreaterThanOrEqualTo(0).When(x => x.QualificationScore.HasValue);
        RuleFor(x => x.CalculationMethod).IsInEnum();
    }
}
