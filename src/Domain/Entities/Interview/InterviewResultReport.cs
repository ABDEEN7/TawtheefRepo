using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewResultReport), Schema = Schemas.Interview)]
public class InterviewResultReport : EventEntity
{
    // Both are database-generated and never set by application code - same pattern as
    // InterviewCommittee: Number comes from the itv.InterviewResultReportNumber sequence and Code is a
    // persisted column derived from it (REP-INT-<year>-<number>).
    public int Number { get; private set; }
    public string Code { get; private set; } = null!;

    public Guid InterviewScheduleId { get; set; }
    public InterviewSchedule? InterviewSchedule { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? AppliedQualificationScore { get; set; }

    public ResultReportStatus Status { get; set; } = ResultReportStatus.Creating;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? DecisionNotes { get; set; }

    // Committee Head Review - who sent the report on to final approval, and when.
    public Guid? CommitteeReviewedById { get; set; }
    public User? CommitteeReviewedBy { get; set; }
    public DateTime? CommitteeReviewedAt { get; set; }

    public ICollection<InterviewResultCandidate> Candidates { get; set; } = [];

    // Built only by InterviewResultCalculationService, never via a user-facing command - Creating is
    // transient in-memory only within that same call, exactly like InterviewSchedule's Draft/Proposed.
    public static InterviewResultReport Create(Guid interviewScheduleId, decimal? appliedQualificationScore)
    {
        return new InterviewResultReport
        {
            InterviewScheduleId = interviewScheduleId,
            AppliedQualificationScore = appliedQualificationScore,
            Status = ResultReportStatus.Creating
        };
    }

    // Called once, immediately after Create() and after all Candidates/Axes are attached, within the
    // same generation call - callers never observe a persisted Creating report. A new report first
    // goes to the Committee Head (chair) review; only SendForApproval moves it on to UnderReview.
    public Result MarkReadyForCommitteeReview()
    {
        if (Status != ResultReportStatus.Creating)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotUnderReview));

        Status = ResultReportStatus.CommitteeReview;
        return Result.Ok();
    }

    // The chair may keep editing the recommendation after sending it, until the approver approves.
    [NotMapped]
    public bool IsCommitteeReviewEditable => Status is ResultReportStatus.CommitteeReview or ResultReportStatus.UnderReview;

    public Result SendForApproval(Guid reviewerId)
    {
        if (Status != ResultReportStatus.CommitteeReview)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotInCommitteeReview));

        Status = ResultReportStatus.UnderReview;
        CommitteeReviewedById = reviewerId;
        CommitteeReviewedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    // Approved/Closed reports have already pushed their final decisions onto the candidates' invitations,
    // so they can never be replaced. Anything earlier (still being reviewed) can be discarded - soft
    // deleted - when the candidate set changes, and the schedule regenerates a fresh report once done.
    [NotMapped]
    public bool IsFinalized => Status is ResultReportStatus.Approved or ResultReportStatus.Closed;

    // Cross-cutting checks the entity cannot perform itself (operational-issue blocking, the
    // Invitation-status sync) live in the handler - this only owns the report+candidate state machine.
    public Result Approve(Guid approverId, IReadOnlyDictionary<Guid, (FinalDecision Decision, string? Reason)> decisions)
    {
        if (Status != ResultReportStatus.UnderReview)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotUnderReview));

        foreach (var candidate in Candidates)
        {
            if (!decisions.TryGetValue(candidate.Id, out var decision))
                return Result.Fail(new Error(ErrorsCodes.InterviewResultCandidateDecisionMissing));

            var decideResult = candidate.Decide(approverId, decision.Decision, decision.Reason);
            if (decideResult.IsFailed)
                return decideResult;
        }

        Status = ResultReportStatus.Approved;
        ApprovedById = approverId;
        ApprovedAt = DateTime.UtcNow;
        return Result.Ok();
    }
}

public enum ResultReportStatus
{
    // Declared in lifecycle order. The numbers are stored in the database, so they are never renumbered:
    Creating = 1,
    CommitteeReview = 2,
    UnderReview = 3,
    Returned = 4,
    Approved = 5,
    Closed = 6
}
