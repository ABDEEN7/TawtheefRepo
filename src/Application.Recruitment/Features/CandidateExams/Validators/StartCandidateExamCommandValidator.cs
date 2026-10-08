using Application.Recruitment.Features.CandidateExams.Commands;
using FluentValidation;

namespace Application.Recruitment.Features.CandidateExams.Validators;

public sealed class StartCandidateExamCommandValidator : AbstractValidator<StartCandidateExamCommand>
{
    public StartCandidateExamCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TestSessionCandidateId).NotEmpty();
        RuleFor(x => x.AccessCode).NotEmpty();
    }
}
