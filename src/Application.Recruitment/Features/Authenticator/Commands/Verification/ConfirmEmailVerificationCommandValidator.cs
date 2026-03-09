using Application.Recruitment.Common.Validation;
using FluentValidation;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public sealed class ConfirmEmailVerificationCommandValidator : AbstractValidator<ConfirmEmailVerificationCommand>
{
    public ConfirmEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(InputValidationPatterns.Email)
            .WithMessage("Email contains invalid characters.");
    }
}
