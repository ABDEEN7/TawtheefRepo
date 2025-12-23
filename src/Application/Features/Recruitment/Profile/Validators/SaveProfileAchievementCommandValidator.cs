using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfileAchievementCommandValidator : AbstractValidator<SaveProfileAchievementCommand>
{
    public SaveProfileAchievementCommandValidator()
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