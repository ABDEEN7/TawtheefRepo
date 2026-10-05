using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class GetAvailableSlotsForRescheduleQueryValidator : AbstractValidator<GetAvailableSlotsForRescheduleQuery>
{
    public GetAvailableSlotsForRescheduleQueryValidator()
    {
        RuleFor(x => x.InterviewScheduleId).NotEmpty();
        RuleFor(x => x.AppointmentId).NotEmpty();
    }
}
