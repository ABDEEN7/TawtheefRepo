using Application.Recruitment.Features.Profile.Command.SaveOperation;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Validators;

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

                RuleFor(x => x.Request.DegreeFiles)
                    .Must(FileValidationHelpers.AreAllAllowedFileTypes)
                    .WithMessage(ErrorsCodes.InvalidFileType);
            });
    }
}
