using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfileExperienceCommandValidator : AbstractValidator<SaveProfileExperienceCommand>
{
    public SaveProfileExperienceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.ExperiencesJson)
                    .NotEmpty()
                    .WithMessage(ErrorsCodes.ExperienceRequired);

                RuleFor(x => x.Request.TrainingCoursesJson).NotNull();
                RuleFor(x => x.Request.ExperienceFiles).NotNull();
                RuleFor(x => x.Request.TrainingCourseFiles).NotNull();

                RuleForEach(x => x.Request.ExperienceFiles)
                    .Must(file => file is null || file.Length <= ProfileLimits.MaxExperienceFileSizeBytes)
                    .WithMessage(ErrorsCodes.ExperienceFileTooLarge);

                RuleForEach(x => x.Request.TrainingCourseFiles)
                    .Must(file => file is null || file.Length <= ProfileLimits.MaxTrainingFileSizeBytes)
                    .WithMessage(ErrorsCodes.TrainingCourseFileTooLarge);
            });
    }
}