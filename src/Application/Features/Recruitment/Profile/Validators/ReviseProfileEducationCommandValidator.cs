using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

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