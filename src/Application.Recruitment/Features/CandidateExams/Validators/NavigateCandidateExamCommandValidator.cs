using Application.Recruitment.Features.CandidateExams.Commands;
using FluentValidation;

namespace Application.Recruitment.Features.CandidateExams.Validators;

public sealed class NavigateCandidateExamCommandValidator : AbstractValidator<NavigateCandidateExamCommand>
{
    public NavigateCandidateExamCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TestAttemptId).NotEmpty();
        RuleFor(x => x.TargetQuestionNumber).GreaterThan(0);
    }
}
