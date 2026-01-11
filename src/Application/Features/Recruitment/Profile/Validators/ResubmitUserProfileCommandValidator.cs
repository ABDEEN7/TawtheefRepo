using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class ResubmitUserProfileCommandValidator : AbstractValidator<ResubmitUserProfileCommand>
{
    public ResubmitUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();
    }
}