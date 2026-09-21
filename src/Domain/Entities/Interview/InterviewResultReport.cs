using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewResultReport), Schema = Schemas.Interview)]
public class InterviewResultReport : EventEntity
{
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
    // same generation call - callers never observe a persisted Creating report.
    public Result MarkReadyForReview()
    {
        if (Status != ResultReportStatus.Creating)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotUnderReview));

        Status = ResultReportStatus.UnderReview;
        return Result.Ok();
    }

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
    Creating = 1,
    UnderReview = 2,
    Returned = 3,
    Approved = 4,
    Closed = 5
}
