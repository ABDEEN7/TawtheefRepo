using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Validators;

public sealed class UpdateTemplateCommandValidator : AbstractValidator<UpdateTemplateCommand>
{
    public UpdateTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TitleEn).MaximumLength(200);
    }
}
