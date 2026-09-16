using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.NewEndAt).GreaterThan(x => x.NewStartAt);
        RuleFor(x => x.RemoteMeetingUrl).MaximumLength(1000);
        RuleFor(x => x.RemoteMeetingInstructions).MaximumLength(1000);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
