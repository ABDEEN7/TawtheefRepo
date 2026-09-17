using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class PeriodInputDtoValidator : AbstractValidator<PeriodInputDto>
{
    public PeriodInputDtoValidator()
    {
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        RuleFor(x => x.RemoteMeetingUrl).MaximumLength(1000);
        RuleFor(x => x.RemoteMeetingInstructions).MaximumLength(1000);
    }
}
