using Application.Recruitment.Features.Profile.Command.SaveOperation;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Validators;

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

                RuleForEach(x => x.Request.AchievementFiles)
                    .Must(FileValidationHelpers.IsAllowedFileType)
                    .WithMessage(ErrorsCodes.InvalidFileType);
            });
    }
}
