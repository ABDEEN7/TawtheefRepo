using Application.Operation.Features.Employee.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class StopCommitteeCommandValidator : AbstractValidator<StopCommitteeCommand>
{
    public StopCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}
