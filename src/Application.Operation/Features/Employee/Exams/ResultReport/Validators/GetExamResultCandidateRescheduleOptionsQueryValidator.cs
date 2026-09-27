using Application.Operation.Features.Employee.Exams.ResultReport.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.ResultReport.Validators;

public sealed class GetExamResultCandidateRescheduleOptionsQueryValidator
    : AbstractValidator<GetExamResultCandidateRescheduleOptionsQuery>
{
    public GetExamResultCandidateRescheduleOptionsQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.CandidateResultId).NotEmpty();
    }
}
