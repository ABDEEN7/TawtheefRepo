using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperations;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

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