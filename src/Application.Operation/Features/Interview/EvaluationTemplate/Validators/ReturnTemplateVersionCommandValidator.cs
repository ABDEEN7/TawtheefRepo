using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class ReturnTemplateVersionCommandValidator : AbstractValidator<ReturnTemplateVersionCommand>
{
    public ReturnTemplateVersionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
