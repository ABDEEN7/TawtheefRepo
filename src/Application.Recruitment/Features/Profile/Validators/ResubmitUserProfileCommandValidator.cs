using Application.Recruitment.Features.Profile.Command;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ResubmitUserProfileCommandValidator : AbstractValidator<ResubmitUserProfileCommand>
{
    public ResubmitUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();
    }
}
