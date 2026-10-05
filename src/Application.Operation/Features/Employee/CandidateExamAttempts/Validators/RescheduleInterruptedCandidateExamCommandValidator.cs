using Application.Operation.Features.Employee.CandidateExamAttempts.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Validators;

public sealed class
    RescheduleInterruptedCandidateExamCommandValidator : AbstractValidator<RescheduleInterruptedCandidateExamCommand>
{
    public RescheduleInterruptedCandidateExamCommandValidator()
    {
        RuleFor(x => x.TestAttemptId).NotEmpty();
        RuleFor(x => x.OfficerId).NotEmpty();
        RuleFor(x => x.TargetTestSessionId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty()
            .Must(reason => !string.IsNullOrWhiteSpace(reason))
            .MaximumLength(2000);
    }
}
