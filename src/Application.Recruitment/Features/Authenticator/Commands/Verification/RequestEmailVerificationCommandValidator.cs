using Application.Recruitment.Common.Validation;
using FluentValidation;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public sealed class RequestEmailVerificationCommandValidator : AbstractValidator<RequestEmailVerificationCommand>
{
    public RequestEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(InputValidationPatterns.Email)
            .WithMessage("Email contains invalid characters.");
    }
}
