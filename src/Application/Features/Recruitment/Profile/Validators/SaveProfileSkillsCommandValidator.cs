using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfileSkillsCommandValidator : AbstractValidator<SaveProfileSkillsCommand>
{
    public SaveProfileSkillsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleForEach(x => x.Request.Skills)
                    .ChildRules(skill =>
                    {
                        skill.RuleFor(s => s.SkillId).NotEmpty();
                        skill.RuleFor(s => s.LevelId).NotEmpty();
                    });
            });
    }
}