using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ReviseProfileSkillsCommandValidator : AbstractValidator<ReviseProfileSkillsCommand>
{
    public ReviseProfileSkillsCommandValidator()
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
