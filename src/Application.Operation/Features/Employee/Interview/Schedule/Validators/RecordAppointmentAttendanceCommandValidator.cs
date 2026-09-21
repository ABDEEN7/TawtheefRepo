using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class RecordAppointmentAttendanceCommandValidator : AbstractValidator<RecordAppointmentAttendanceCommand>
{
    public RecordAppointmentAttendanceCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.AttendanceStatus).IsInEnum();
    }
}
