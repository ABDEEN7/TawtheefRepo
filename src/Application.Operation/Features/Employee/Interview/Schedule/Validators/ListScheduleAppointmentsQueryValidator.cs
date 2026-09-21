using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class ListScheduleAppointmentsQueryValidator : AbstractValidator<ListScheduleAppointmentsQuery>
{
    public ListScheduleAppointmentsQueryValidator()
    {
        RuleFor(x => x.InterviewScheduleId).NotEmpty();
    }
}
