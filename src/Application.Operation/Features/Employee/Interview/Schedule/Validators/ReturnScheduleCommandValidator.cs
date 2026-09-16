using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class ReturnScheduleCommandValidator : AbstractValidator<ReturnScheduleCommand>
{
    public ReturnScheduleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
