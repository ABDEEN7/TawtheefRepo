using Application.Operation.Features.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.Committee.Validators;

public sealed class ReturnCommitteeCommandValidator : AbstractValidator<ReturnCommitteeCommand>
{
    public ReturnCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
