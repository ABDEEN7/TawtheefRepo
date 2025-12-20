using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfileLanguagesCommandValidator : AbstractValidator<SaveProfileLanguagesCommand>
{
    public SaveProfileLanguagesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleForEach(x => x.Request.Languages)
                    .ChildRules(lang =>
                    {
                        lang.RuleFor(l => l.LanguageId).NotEmpty();
                        lang.RuleFor(l => l.SpeakingLevelId).NotEmpty();
                        lang.RuleFor(l => l.WritingLevelId).NotEmpty();
                        lang.RuleFor(l => l.ReadingLevelId).NotEmpty();
                    });

                RuleFor(x => x.Request)
                    .Must(r => (r.Languages.Count) > 0)
                    .WithMessage(ErrorsCodes.SkillOrLanguageRequired);
            });
    }
}