using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewResultCandidate), Schema = Schemas.Interview)]
public class InterviewResultCandidate : EventEntity
{
    public Guid InterviewResultReportId { get; set; }
    public InterviewResultReport? InterviewResultReport { get; set; }

    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal FinalScore { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? QualificationScore { get; set; }

    public bool IsQualified { get; set; }
    public CalculationMethod CalculationMethod { get; set; }

    public FinalDecision? FinalDecision { get; set; }
    public string? DecisionReason { get; set; }
    public Guid? DecidedById { get; set; }
    public User? DecidedBy { get; set; }
    public DateTime? DecidedAt { get; set; }

    public DateTime SnapshotAt { get; set; }

    // Committee Head Review. ChairRecommendedDecision overrides the system suggestion and pre-fills
    // the approver's dropdown; null means "go with the suggestion". RecommendedSchoolStageId is
    // informational only - it never feeds any score, qualification or suggestion.
    public FinalDecision? ChairRecommendedDecision { get; set; }
    public string? ChairRecommendationReason { get; set; }
    public Guid? RecommendedSchoolStageId { get; set; }
    public SchoolStage? RecommendedSchoolStage { get; set; }

    public ICollection<InterviewResultCandidateAxis> Axes { get; set; } = [];

    // Built only by the calculation engine (InterviewResultCalculationService), never via a command -
    // FinalScore/QualificationScore/IsQualified/CalculationMethod/Axes are all frozen at generation time.
    public static InterviewResultCandidate Create(
        Guid interviewAppointmentId, decimal finalScore, decimal? qualificationScore,
        bool isQualified, CalculationMethod calculationMethod, DateTime snapshotAt)
    {
        return new InterviewResultCandidate
        {
            InterviewAppointmentId = interviewAppointmentId,
            FinalScore = finalScore,
            QualificationScore = qualificationScore,
            IsQualified = isQualified,
            CalculationMethod = calculationMethod,
            SnapshotAt = snapshotAt
        };
    }

    // Guarded by the parent report's status rather than its own - a candidate row has no independent
    // lifecycle, it can only be decided while the report as a whole is still under review.
    // Unqualified candidates can only be Rejected - CandidateForHiringProcess/WaitingList require
    // IsQualified, matching the (previously dead, frontend-only).
    public Result Decide(Guid decidedById, FinalDecision decision, string? reason)
    {
        if (InterviewResultReport!.Status != ResultReportStatus.UnderReview)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotUnderReview));

        if (!IsQualified && decision is Interview.FinalDecision.CandidateForHiringProcess or Interview.FinalDecision.WaitingList)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultCandidateNotQualifiedForDecision));

        FinalDecision = decision;
        DecisionReason = reason;
        DecidedById = decidedById;
        DecidedAt = DateTime.UtcNow;
        SnapshotAt = DecidedAt.Value;
        return Result.Ok();
    }

    // Same qualification rule as Decide(): the chair can't recommend hiring/waiting list for an
    // unqualified candidate. Passing null clears the override back to the system suggestion.
    public Result SetChairRecommendation(FinalDecision? decision, string? reason)
    {
        if (!InterviewResultReport!.IsCommitteeReviewEditable)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotInCommitteeReview));

        if (!IsQualified && decision is Interview.FinalDecision.CandidateForHiringProcess or Interview.FinalDecision.WaitingList)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultCandidateNotQualifiedForDecision));

        ChairRecommendedDecision = decision;
        ChairRecommendationReason = decision is null ? null : reason;
        return Result.Ok();
    }

    public Result SetRecommendedSchoolStage(Guid? schoolStageId)
    {
        if (!InterviewResultReport!.IsCommitteeReviewEditable)
            return Result.Fail(new Error(ErrorsCodes.InterviewResultReportNotInCommitteeReview));

        RecommendedSchoolStageId = schoolStageId;
        return Result.Ok();
    }
}

public enum FinalDecision
{
    CandidateForHiringProcess = 1,
    WaitingList = 2,
    Rejected = 3,
    NeedsAction = 4,
    NoShow = 5
}
