using Application.Recruitment.Features.CandidateExams.Commands;
using FluentValidation;

namespace Application.Recruitment.Features.CandidateExams.Validators;

public sealed class CompleteCandidateExamPartCommandValidator : AbstractValidator<SubmitCandidateExamPartCommand>
{
    public CompleteCandidateExamPartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TestAttemptId).NotEmpty();
        RuleFor(x => x.TestAttemptPartId).NotEmpty();
    }
}
