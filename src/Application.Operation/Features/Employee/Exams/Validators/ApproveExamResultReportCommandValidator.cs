using Application.Operation.Features.Employee.Exams.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class ApproveExamResultReportCommandValidator
    : AbstractValidator<ApproveExamResultReportCommand>
{
    public ApproveExamResultReportCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty();

        RuleFor(x => x.DecisionNotes)
            .MaximumLength(2000);
    }
}
