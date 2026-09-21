using Application.Operation.Features.Employee.Interview.Evaluation.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Validators;

public sealed class SaveMemberEvaluationDraftCommandValidator : AbstractValidator<SaveMemberEvaluationDraftCommand>
{
    public SaveMemberEvaluationDraftCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.Scores).NotEmpty();
        RuleForEach(x => x.Scores).ChildRules(score =>
        {
            score.RuleFor(s => s.CriterionId).NotEmpty();
            score.RuleFor(s => s.Score).GreaterThanOrEqualTo(0);
            score.RuleFor(s => s.Notes).MaximumLength(2000);
        });
        RuleFor(x => x.GeneralNotes).MaximumLength(2000);
    }
}
