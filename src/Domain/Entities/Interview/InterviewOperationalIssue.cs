using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewOperationalIssue), Schema = Schemas.Interview)]
public class InterviewOperationalIssue : EventEntity
{
    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    public OperationalIssueType IssueType { get; set; }
    public string? Description { get; set; }
    public bool IsBlocking { get; set; }
    public OperationalIssueStatus Status { get; set; } = OperationalIssueStatus.Open;

    public Guid? ResolvedById { get; set; }
    public User? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNotes { get; set; }

    // No status/lifecycle restriction on the appointment itself - an issue can be logged before,
    // during, or after the interview, per the BRD.
    public static InterviewOperationalIssue Create(
        Guid interviewAppointmentId, OperationalIssueType issueType, string? description, bool isBlocking)
    {
        return new InterviewOperationalIssue
        {
            InterviewAppointmentId = interviewAppointmentId,
            IssueType = issueType,
            Description = description,
            IsBlocking = isBlocking,
            Status = OperationalIssueStatus.Open
        };
    }

    public Result Resolve(Guid resolvedById, string? notes)
    {
        if (Status != OperationalIssueStatus.Open)
            return Result.Fail(new Error(ErrorsCodes.InterviewOperationalIssueNotOpen));

        Status = OperationalIssueStatus.Resolved;
        ResolvedById = resolvedById;
        ResolvedAt = DateTime.UtcNow;
        ResolutionNotes = notes;
        return Result.Ok();
    }

    // Waived carries the same gate-clearing effect as Resolved (both take Status out of Open) - it's
    // a different paper trail ("acknowledged, doesn't actually need to block"), not a weaker resolution.
    public Result Waive(Guid resolvedById, string? notes)
    {
        if (Status != OperationalIssueStatus.Open)
            return Result.Fail(new Error(ErrorsCodes.InterviewOperationalIssueNotOpen));

        Status = OperationalIssueStatus.Waived;
        ResolvedById = resolvedById;
        ResolvedAt = DateTime.UtcNow;
        ResolutionNotes = notes;
        return Result.Ok();
    }

    // Lets a Final Reviewer explicitly flag (or unflag) an issue as blocking during review, distinct
    // from whoever logged it at creation time - only while still Open, matching Resolve/Waive's guard.
    public Result UpdateBlocking(bool isBlocking)
    {
        if (Status != OperationalIssueStatus.Open)
            return Result.Fail(new Error(ErrorsCodes.InterviewOperationalIssueNotOpen));

        IsBlocking = isBlocking;
        return Result.Ok();
    }
}

public enum OperationalIssueType
{
    NoShow = 1,
    CandidateWithdrawal = 2,
    IncompleteEvaluation = 3,
    TechnicalProblem = 4,
    CouldNotBeConducted = 5,
    RescheduleRequest = 6
}

public enum OperationalIssueStatus
{
    Open = 1,
    Resolved = 2,
    Waived = 3
}
