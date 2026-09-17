using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using Application.Operation.Features.Employee.Interview.Schedule.Validators;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class PreviewScheduleSlotsQueryValidator : AbstractValidator<PreviewScheduleSlotsQuery>
{
    public PreviewScheduleSlotsQueryValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.InterviewType).IsInEnum();
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(ScheduleConstants.MinimumDurationMinutes);
        RuleFor(x => x.BufferMinutes).GreaterThanOrEqualTo(ScheduleConstants.MinimumBufferMinutes);
        RuleFor(x => x.Periods).NotEmpty();
        RuleForEach(x => x.Periods).SetValidator(new PeriodInputDtoValidator());
        RuleForEach(x => x.ManualAssignments).SetValidator(new SlotAssignmentDtoValidator());
    }
}
