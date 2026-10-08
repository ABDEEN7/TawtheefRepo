using Application.Recruitment.Features.CandidateExams.Commands;
using FluentValidation;

namespace Application.Recruitment.Features.CandidateExams.Validators;

public sealed class SaveCandidateExamAnswerCommandValidator : AbstractValidator<SaveCandidateExamAnswerCommand>
{
    public SaveCandidateExamAnswerCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TestAttemptId).NotEmpty();
        RuleFor(x => x.TestAttemptQuestionId).NotEmpty();
        RuleFor(x => x.SelectedOptionId).NotEmpty();
    }
}
