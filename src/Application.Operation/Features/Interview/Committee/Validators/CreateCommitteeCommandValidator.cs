using Application.Operation.Features.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.Committee.Validators;

public sealed class CreateCommitteeCommandValidator : AbstractValidator<CreateCommitteeCommand>
{
    public CreateCommitteeCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.InterviewTemplateId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).MaximumLength(200);
        RuleFor(x => x.ScopeDescription).MaximumLength(1000);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
