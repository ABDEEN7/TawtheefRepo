using Application.Operation.Features.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.Committee.Validators;

public sealed class UpdateCommitteeCommandValidator : AbstractValidator<UpdateCommitteeCommand>
{
    public UpdateCommitteeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).MaximumLength(200);
        RuleFor(x => x.ScopeDescription).MaximumLength(1000);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
