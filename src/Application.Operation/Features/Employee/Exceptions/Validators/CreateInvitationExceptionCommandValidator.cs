using Application.Operation.Features.Employee.Exceptions.Commands;
using FluentValidation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Validators;

public sealed class CreateInvitationExceptionCommandValidator
    : AbstractValidator<CreateInvitationExceptionCommand>
{
    public CreateInvitationExceptionCommandValidator()
    {
        RuleFor(command => command.Qid)
            .NotEmpty()
            .WithErrorCode(ErrorsCodes.InvalidQidFormat)
            .WithMessage(ErrorsCodes.InvalidQidFormat);

        RuleFor(command => command.JobId)
            .NotEmpty()
            .WithErrorCode(JobMessages.JobNotFound)
            .WithMessage(JobMessages.JobNotFound);

        RuleFor(command => command.Reason)
            .NotEmpty()
            .WithErrorCode(ErrorsCodes.ExceptionReasonRequired)
            .WithMessage(ErrorsCodes.ExceptionReasonRequired)
            .MaximumLength(InvitationException.ReasonMaxLength)
            .WithErrorCode(ErrorsCodes.ExceptionReasonTooLong)
            .WithMessage(ErrorsCodes.ExceptionReasonTooLong);

        RuleFor(command => command.Proof)
            .NotNull()
            .WithErrorCode(ErrorsCodes.EmptyFile)
            .WithMessage(ErrorsCodes.EmptyFile)
            .Must(proof => proof is { Length: > 0 })
            .WithErrorCode(ErrorsCodes.EmptyFile)
            .WithMessage(ErrorsCodes.EmptyFile);
    }
}
