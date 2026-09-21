using Application.Operation.Features.Employee.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class CancelCommitteeCommandValidator : AbstractValidator<CancelCommitteeCommand>
{
    public CancelCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
