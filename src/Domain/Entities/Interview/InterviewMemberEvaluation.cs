using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewMemberEvaluation), Schema = Schemas.Interview)]
public class InterviewMemberEvaluation : EventEntity
{
    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    public Guid InterviewCommitteeMemberId { get; set; }
    public InterviewCommitteeMember? InterviewCommitteeMember { get; set; }

    public MemberEvaluationStatus Status { get; set; } = MemberEvaluationStatus.Draft;

    [Column(TypeName = "decimal(6,2)")]
    public decimal? TotalScore { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ReopenedById { get; set; }
    public User? ReopenedBy { get; set; }
    public DateTime? ReopenedAt { get; set; }
    public string? ReopenReason { get; set; }

    public string? GeneralNotes { get; set; }

    public ICollection<InterviewMemberEvaluationCriterion> CriterionScores { get; set; } = [];

    // The caller (command handler) must have already verified the appointment is in an evaluable
    // state and that the committee member is the one actually assigned/allowed to score it -
    // cross-aggregate checks this entity cannot perform itself.
    public static InterviewMemberEvaluation Create(Guid interviewAppointmentId, Guid interviewCommitteeMemberId)
    {
        return new InterviewMemberEvaluation
        {
            InterviewAppointmentId = interviewAppointmentId,
            InterviewCommitteeMemberId = interviewCommitteeMemberId,
            Status = MemberEvaluationStatus.Draft
        };
    }

    // Reopen is deferred (fields exist, no flow built yet), so Draft is the only editable state -
    // once Submitted this always fails rather than silently overwriting a submitted score.
    // Fully syncs CriterionScores to the incoming set each call (add new, update existing, drop ones
    // no longer sent) - mirrors the InterviewCommitteeMember.SetEvaluationAxes full-replace convention,
    // since the caller always resends the complete form rather than one criterion at a time.
    public Result SaveDraft(IEnumerable<MemberEvaluationCriterionScoreInput> scores, string? generalNotes)
    {
        if (Status != MemberEvaluationStatus.Draft)
            return Result.Fail(new Error(ErrorsCodes.InterviewMemberEvaluationAlreadySubmitted));

        var incoming = scores.ToList();
        foreach (var score in incoming)
        {
            if (score.Score < 0 || score.Score > score.MaxScore)
                return Result.Fail(new Error(ErrorsCodes.InterviewMemberEvaluationScoreOutOfRange));
        }

        var incomingIds = incoming.Select(s => s.CriterionId).ToHashSet();
        foreach (var stale in CriterionScores.Where(c => !incomingIds.Contains(c.InterviewTemplateEvaluationCriterionId)).ToList())
            CriterionScores.Remove(stale);

        foreach (var score in incoming)
        {
            var existing = CriterionScores.FirstOrDefault(c => c.InterviewTemplateEvaluationCriterionId == score.CriterionId);
            if (existing is not null)
            {
                existing.Score = score.Score;
                existing.Notes = score.Notes;
                continue;
            }

            CriterionScores.Add(new InterviewMemberEvaluationCriterion
            {
                InterviewMemberEvaluationId = Id,
                InterviewTemplateEvaluationCriterionId = score.CriterionId,
                Score = score.Score,
                Notes = score.Notes
            });
        }

        GeneralNotes = generalNotes;
        return Result.Ok();
    }

    // requiredCriterionIds is the required-criterion set within this member's scope (all axes, or
    // just their SelectedAxes) - the handler computes it from the template version, this method
    // only checks it against what's actually been scored.
    public Result Submit(IReadOnlyCollection<Guid> requiredCriterionIds)
    {
        if (Status != MemberEvaluationStatus.Draft)
            return Result.Fail(new Error(ErrorsCodes.InterviewMemberEvaluationAlreadySubmitted));

        var scoredIds = CriterionScores.Select(c => c.InterviewTemplateEvaluationCriterionId).ToHashSet();
        if (requiredCriterionIds.Any(id => !scoredIds.Contains(id)))
            return Result.Fail(new Error(ErrorsCodes.InterviewMemberEvaluationRequiredCriterionMissing));

        TotalScore = CriterionScores.Sum(c => c.Score);
        Status = MemberEvaluationStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        return Result.Ok();
    }
}

// One criterion score from the member's scoring form. Not a MediatR command, just SaveDraft's input
// shape - MaxScore is passed in by the handler (loaded from InterviewTemplateEvaluationCriterion)
// since this entity doesn't own that data itself.
public sealed record MemberEvaluationCriterionScoreInput(Guid CriterionId, decimal Score, decimal MaxScore, string? Notes);

public enum MemberEvaluationStatus
{
    Draft = 1,
    Submitted = 2,
    Reopened = 3
}
