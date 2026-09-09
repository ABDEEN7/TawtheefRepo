using Application.Operation.Features.Interview.EvaluationBank.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationBank.Validators;

public sealed class UpdateAxisCommandValidator : AbstractValidator<UpdateAxisCommand>
{
    public UpdateAxisCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).MaximumLength(200);
    }
}
