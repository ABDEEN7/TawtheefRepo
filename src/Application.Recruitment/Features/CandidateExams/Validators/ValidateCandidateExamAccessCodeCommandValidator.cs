using Application.Recruitment.Features.CandidateExams.Commands;
using FluentValidation;

namespace Application.Recruitment.Features.CandidateExams.Validators;

public sealed class ValidateCandidateExamAccessCodeCommandValidator
    : AbstractValidator<ValidateCandidateExamAccessCodeCommand>
{
    public ValidateCandidateExamAccessCodeCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TestSessionCandidateId).NotEmpty();
        RuleFor(command => command.AccessCode).NotEmpty();
    }
}
