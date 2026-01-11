using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

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