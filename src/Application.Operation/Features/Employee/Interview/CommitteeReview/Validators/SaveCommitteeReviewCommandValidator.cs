using Application.Operation.Features.Employee.Interview.CommitteeReview.Commands;
using FluentValidation;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Validators;

public sealed class SaveCommitteeReviewCommandValidator : AbstractValidator<SaveCommitteeReviewCommand>
{
    public SaveCommitteeReviewCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.Candidates).NotNull();
        RuleForEach(x => x.Candidates).ChildRules(candidate =>
        {
            candidate.RuleFor(c => c.CandidateId).NotEmpty();
            // Same 3 decisions the approval dropdown offers.
            candidate.RuleFor(c => c.RecommendedDecision).Must(d => d is null
                or FinalDecision.CandidateForHiringProcess or FinalDecision.WaitingList or FinalDecision.Rejected);
            candidate.RuleFor(c => c.Reason).MaximumLength(2000);
        });
    }
}
