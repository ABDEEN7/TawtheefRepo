using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ReviseProfileAttachmentsCommandValidator : AbstractValidator<ReviseProfileAttachmentsCommand>
{
    public ReviseProfileAttachmentsCommandValidator()
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
