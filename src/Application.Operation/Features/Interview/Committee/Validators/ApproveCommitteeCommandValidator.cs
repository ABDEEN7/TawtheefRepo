using Application.Operation.Features.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.Committee.Validators;

public sealed class ApproveCommitteeCommandValidator : AbstractValidator<ApproveCommitteeCommand>
{
    public ApproveCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DecisionNotes).MaximumLength(1000);
    }
}
