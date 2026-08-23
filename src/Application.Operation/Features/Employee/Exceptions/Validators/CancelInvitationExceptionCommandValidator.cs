using Application.Operation.Features.Employee.Exceptions.Commands;
using FluentValidation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Validators;

public sealed class CancelInvitationExceptionCommandValidator
    : AbstractValidator<CancelInvitationExceptionCommand>
{
    public CancelInvitationExceptionCommandValidator()
    {
        RuleFor(command => command.Reason)
            .NotEmpty()
            .WithErrorCode(ErrorsCodes.ExceptionReasonRequired)
            .WithMessage(ErrorsCodes.ExceptionReasonRequired)
            .MaximumLength(InvitationException.ReasonMaxLength)
            .WithErrorCode(ErrorsCodes.ExceptionReasonTooLong)
            .WithMessage(ErrorsCodes.ExceptionReasonTooLong);
    }
}
