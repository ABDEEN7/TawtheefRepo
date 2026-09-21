using Application.Operation.Features.Employee.Interview.ResultReport.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Validators;

public sealed class GetResultReportByScheduleQueryValidator : AbstractValidator<GetResultReportByScheduleQuery>
{
    public GetResultReportByScheduleQueryValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
    }
}
