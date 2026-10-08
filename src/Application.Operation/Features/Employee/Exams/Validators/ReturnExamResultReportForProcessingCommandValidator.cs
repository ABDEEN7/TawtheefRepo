using Application.Operation.Features.Employee.Exams.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class ReturnExamResultReportForProcessingCommandValidator
    : AbstractValidator<ReturnExamResultReportForProcessingCommand>
{
    public ReturnExamResultReportForProcessingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().Must(reason => !string.IsNullOrWhiteSpace(reason))
            .MaximumLength(2000);
    }
}
