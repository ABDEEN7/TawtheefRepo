using Application.Operation.Features.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.Committee.Validators;

public sealed class CancelCommitteeCommandValidator : AbstractValidator<CancelCommitteeCommand>
{
    public CancelCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
