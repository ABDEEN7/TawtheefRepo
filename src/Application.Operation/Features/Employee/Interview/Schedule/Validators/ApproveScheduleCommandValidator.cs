using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class ApproveScheduleCommandValidator : AbstractValidator<ApproveScheduleCommand>
{
    public ApproveScheduleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DecisionNotes).MaximumLength(1000);
    }
}
