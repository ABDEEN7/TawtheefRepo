using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestAttempt), Schema = Schemas.Hr)]
public class TestAttempt : EventEntity
{
    public Guid TestSessionCandidateId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? ActiveSessionId { get; set; }

    public void Start(Guid activeSessionId, DateTime startedAt)
    {
        if (StatusId != TestAttemptStatusIds.NotStarted || StartedAt is not null || CompletedAt is not null
            || ActiveSessionId is not null)
        {
            throw new InvalidOperationException("Only a not-started attempt can be started.");
        }

        if (activeSessionId == Guid.Empty)
        {
            throw new ArgumentException("Active session id cannot be empty.", nameof(activeSessionId));
        }

        StatusId = TestAttemptStatusIds.InProgress;
        StartedAt = startedAt;
        ActiveSessionId = activeSessionId;
    }

    public void Interrupt()
    {
        if (StatusId != TestAttemptStatusIds.InProgress || StartedAt is null || CompletedAt is not null
            || ActiveSessionId is null)
        {
            throw new InvalidOperationException("Only an active in-progress attempt can be interrupted.");
        }

        StatusId = TestAttemptStatusIds.Interrupted;
        ActiveSessionId = null;
    }

    public void Resume(Guid activeSessionId)
    {
        if (StatusId != TestAttemptStatusIds.Interrupted || StartedAt is null || CompletedAt is not null
            || ActiveSessionId is not null)
        {
            throw new InvalidOperationException("Only an interrupted attempt can be resumed.");
        }

        if (activeSessionId == Guid.Empty)
        {
            throw new ArgumentException("Active session id cannot be empty.", nameof(activeSessionId));
        }

        StatusId = TestAttemptStatusIds.InProgress;
        ActiveSessionId = activeSessionId;
    }

    public void Complete(DateTime completedAt)
    {
        EnsureCanEnd();

        StatusId = TestAttemptStatusIds.Completed;
        CompletedAt = completedAt;
        ActiveSessionId = null;
    }

    public void MarkTimedOut(DateTime completedAt)
    {
        EnsureCanEnd();

        StatusId = TestAttemptStatusIds.TimedOut;
        CompletedAt = completedAt;
        ActiveSessionId = null;
    }

    public void CancelInterrupted()
    {
        if (StatusId != TestAttemptStatusIds.Interrupted ||
            StartedAt is null ||
            CompletedAt is not null ||
            ActiveSessionId is not null)
        {
            throw new InvalidOperationException(
                "Only a valid interrupted attempt can be cancelled.");
        }

        StatusId = TestAttemptStatusIds.Cancelled;
    }

    private void EnsureCanEnd()
    {
        if (StatusId != TestAttemptStatusIds.InProgress || StartedAt is null || CompletedAt is not null)
        {
            throw new InvalidOperationException("Only an active in-progress attempt can be ended.");
        }
    }

    public TestSessionCandidate? TestSessionCandidate { get; set; }
    public TestAttemptStatus? Status { get; set; }
    public ICollection<TestAttemptPart> Parts { get; set; } = new List<TestAttemptPart>();
    public ICollection<TestAttemptQuestion> Questions { get; set; } = new List<TestAttemptQuestion>();
    public ICollection<CandidateExamConnection> Connections { get; set; } = new List<CandidateExamConnection>();
}
