using Application.Operation.Features.Employee.Interview.ResultReport.Commands;
using FluentValidation;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Validators;

public sealed class ApproveInterviewResultReportCommandValidator : AbstractValidator<ApproveInterviewResultReportCommand>
{
    public ApproveInterviewResultReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.Decisions).NotEmpty();
        RuleForEach(x => x.Decisions).ChildRules(decision =>
        {
            decision.RuleFor(d => d.CandidateId).NotEmpty();
            // Only the 3 decisions scheduleResult.md's dropdown actually offers - NeedsAction/NoShow
            // are real enum members but not part of this approval flow.
            decision.RuleFor(d => d.Decision).Must(d => d is FinalDecision.CandidateForHiringProcess
                or FinalDecision.WaitingList or FinalDecision.Rejected);
            decision.RuleFor(d => d.Reason).MaximumLength(2000);
        });
    }
}
