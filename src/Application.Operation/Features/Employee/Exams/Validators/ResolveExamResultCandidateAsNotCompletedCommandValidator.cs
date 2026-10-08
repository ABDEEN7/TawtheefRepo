using Application.Operation.Features.Employee.Exams.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class ResolveExamResultCandidateAsNotCompletedCommandValidator
    : AbstractValidator<ResolveExamResultCandidateAsNotCompletedCommand>
{
    public ResolveExamResultCandidateAsNotCompletedCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.CandidateResultId).NotEmpty();
        RuleFor(x => x.ResolutionNotes)
            .NotEmpty()
            .Must(notes => !string.IsNullOrWhiteSpace(notes))
            .MaximumLength(2000);
    }
}
