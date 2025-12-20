using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperations;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfileEducationCommandValidator : AbstractValidator<SaveProfileEducationCommand>
{
    public SaveProfileEducationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.DegreesJson).NotEmpty();
            });
    }
}