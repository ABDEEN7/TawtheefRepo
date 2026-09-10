using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Validators;

public sealed class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TitleEn).MaximumLength(200);
    }
}
