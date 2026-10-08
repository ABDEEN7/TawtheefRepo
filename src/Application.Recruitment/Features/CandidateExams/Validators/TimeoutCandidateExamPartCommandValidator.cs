using Application.Recruitment.Features.CandidateExams.Commands;
using FluentValidation;

namespace Application.Recruitment.Features.CandidateExams.Validators;

public sealed class TimeoutCandidateExamPartCommandValidator : AbstractValidator<TimeoutCandidateExamPartCommand>
{
    public TimeoutCandidateExamPartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TestAttemptId).NotEmpty();
        RuleFor(x => x.TestAttemptPartId).NotEmpty();
    }
}
