using Application.Recruitment.Features.Profile.Command.SaveOperation;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class SaveProfileAttachmentsCommandValidator : AbstractValidator<SaveProfileAttachmentsCommand>
{
    public SaveProfileAttachmentsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.AttachmentsJson).NotEmpty();
                RuleFor(x => x.Request.AttachmentFiles).NotNull();

                RuleForEach(x => x.Request.AttachmentFiles)
                    .Must(FileValidationHelpers.IsAllowedFileType)
                    .WithMessage(ErrorsCodes.InvalidFileType);
            });
    }
}
