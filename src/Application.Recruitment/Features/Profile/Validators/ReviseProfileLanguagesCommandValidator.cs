using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ReviseProfileLanguagesCommandValidator : AbstractValidator<ReviseProfileLanguagesCommand>
{
    public ReviseProfileLanguagesCommandValidator()
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
