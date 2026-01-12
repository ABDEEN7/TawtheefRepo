using Application.Recruitment.Features.Profile.Command;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class SubmitUserProfileCommandValidator : AbstractValidator<SubmitUserProfileCommand>
{
    public SubmitUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();
    }
}
