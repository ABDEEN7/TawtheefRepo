using Application.Operation.Features.Employee.Exams.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class GetExamResultReportDetailsQueryValidator
    : AbstractValidator<GetExamResultReportDetailsQuery>
{
    public GetExamResultReportDetailsQueryValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty();
    }
}
