using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class GetScheduleCreationContextQueryValidator : AbstractValidator<GetScheduleCreationContextQuery>
{
    public GetScheduleCreationContextQueryValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
    }
}
