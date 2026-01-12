using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ReviseProfileEducationCommandValidator : AbstractValidator<ReviseProfileEducationCommand>
{
    public ReviseProfileEducationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.DegreesJson).NotEmpty();
            });
    }
}
