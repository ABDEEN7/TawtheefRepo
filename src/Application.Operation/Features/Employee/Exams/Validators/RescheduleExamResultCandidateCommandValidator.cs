using Application.Operation.Features.Employee.Exams.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class RescheduleExamResultCandidateCommandValidator
    : AbstractValidator<RescheduleExamResultCandidateCommand>
{
    public RescheduleExamResultCandidateCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.CandidateResultId).NotEmpty();
        RuleFor(x => x.TargetTestSessionId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty()
            .Must(reason => !string.IsNullOrWhiteSpace(reason))
            .MaximumLength(2000);
    }
}
