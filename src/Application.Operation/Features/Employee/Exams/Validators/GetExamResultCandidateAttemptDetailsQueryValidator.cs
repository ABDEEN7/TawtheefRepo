using Application.Operation.Features.Employee.Exams.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class GetExamResultCandidateAttemptDetailsQueryValidator
    : AbstractValidator<GetExamResultCandidateAttemptDetailsQuery>
{
    public GetExamResultCandidateAttemptDetailsQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.CandidateResultId).NotEmpty();
    }
}
