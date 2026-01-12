using Application.Recruitment.Features.Profile.Command.SaveOperation;
using FluentValidation;

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
            });
    }
}
