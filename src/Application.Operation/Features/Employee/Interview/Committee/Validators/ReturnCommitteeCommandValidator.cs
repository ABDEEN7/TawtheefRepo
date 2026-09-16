using Application.Operation.Features.Employee.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class ReturnCommitteeCommandValidator : AbstractValidator<ReturnCommitteeCommand>
{
    public ReturnCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
