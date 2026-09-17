using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Validators;

public sealed class CancelTemplateVersionCommandValidator : AbstractValidator<CancelTemplateVersionCommand>
{
    public CancelTemplateVersionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
