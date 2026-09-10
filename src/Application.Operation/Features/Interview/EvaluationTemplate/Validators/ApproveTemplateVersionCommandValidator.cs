using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class ApproveTemplateVersionCommandValidator : AbstractValidator<ApproveTemplateVersionCommand>
{
    public ApproveTemplateVersionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DecisionNotes).MaximumLength(1000);
    }
}
