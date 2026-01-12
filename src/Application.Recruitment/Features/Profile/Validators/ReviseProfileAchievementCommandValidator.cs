using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ReviseProfileAchievementCommandValidator : AbstractValidator<ReviseProfileAchievementCommand>
{
    public ReviseProfileAchievementCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.AchievementsJson).NotEmpty();
                RuleFor(x => x.Request.AchievementFiles).NotNull();

                RuleForEach(x => x.Request.AchievementFiles)
                    .Must(file => file is null || file.Length <= ProfileLimits.MaxAchievementFileSizeBytes)
                    .WithMessage(ErrorsCodes.AchievementFileTooLarge);
            });
    }
}
