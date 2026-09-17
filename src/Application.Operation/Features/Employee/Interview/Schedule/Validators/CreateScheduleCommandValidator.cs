using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.Validators;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.InterviewType).IsInEnum();
        RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TitleEn).MaximumLength(200);
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(ScheduleConstants.MinimumDurationMinutes);
        RuleFor(x => x.BufferMinutes).GreaterThanOrEqualTo(ScheduleConstants.MinimumBufferMinutes);

        RuleFor(x => x.Periods).NotEmpty();
        RuleForEach(x => x.Periods).SetValidator(new PeriodInputDtoValidator());
        RuleForEach(x => x.ManualAssignments).SetValidator(new SlotAssignmentDtoValidator());
    }
}
