using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestAttemptPart), Schema = Schemas.Hr)]
public class TestAttemptPart : EventEntity
{
    public Guid TestAttemptId { get; set; }
    public Guid ExamPartId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationMinutesSnapshot { get; set; }
    public DateTime? EffectiveDeadlineAt { get; set; }
    public int? LastQuestionNo { get; set; }

    public bool IsCompletelyNotStarted =>
        StatusId == TestAttemptPartStatusIds.NotStarted && StartedAt is null && CompletedAt is null
        && DurationMinutesSnapshot is null && EffectiveDeadlineAt is null && LastQuestionNo is null;

    public bool HasValidRunningState =>
        StatusId == TestAttemptPartStatusIds.InProgress && StartedAt is not null && CompletedAt is null
        && DurationMinutesSnapshot is > 0 && EffectiveDeadlineAt is not null && LastQuestionNo is > 0;

    public TestAttempt? TestAttempt { get; set; }
    public ExamPart? ExamPart { get; set; }
    public TestAttemptPartStatus? Status { get; set; }
    public ICollection<TestAttemptQuestion> Questions { get; set; } = new List<TestAttemptQuestion>();

    public void Start(DateTime startedAt, int durationMinutes)
    {
        if (!IsCompletelyNotStarted)
        {
            throw new InvalidOperationException("Only a completely not-started attempt part can be started.");
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationMinutes);

        StatusId = TestAttemptPartStatusIds.InProgress;
        StartedAt = startedAt;
        DurationMinutesSnapshot = durationMinutes;
        EffectiveDeadlineAt = startedAt.AddMinutes(durationMinutes);
        LastQuestionNo = 1;
    }

    public void Complete(DateTime completedAt)
    {
        StatusId = TestAttemptPartStatusIds.Completed;
        CompletedAt = completedAt;
    }

    public void MarkTimedOut()
    {
        if (EffectiveDeadlineAt is null)
        {
            throw new InvalidOperationException("An attempt part cannot time out without an effective deadline.");
        }

        StatusId = TestAttemptPartStatusIds.TimedOut;
        CompletedAt = EffectiveDeadlineAt.Value;
    }

    public void Interrupt()
    {
        if (StatusId != TestAttemptPartStatusIds.InProgress)
        {
            throw new InvalidOperationException("Only an in-progress attempt part can be interrupted.");
        }

        if (StartedAt is null || EffectiveDeadlineAt is null || LastQuestionNo is not > 0)
        {
            throw new InvalidOperationException("An in-progress attempt part must have a valid execution state.");
        }

        StatusId = TestAttemptPartStatusIds.Interrupted;
    }

    public void Resume(DateTime newEffectiveDeadlineAt)
    {
        if (StatusId != TestAttemptPartStatusIds.Interrupted)
        {
            throw new InvalidOperationException("Only an interrupted attempt part can be resumed.");
        }

        if (StartedAt is null || EffectiveDeadlineAt is null || LastQuestionNo is not > 0)
        {
            throw new InvalidOperationException("An interrupted attempt part must have a valid execution state.");
        }

        if (newEffectiveDeadlineAt < EffectiveDeadlineAt.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(newEffectiveDeadlineAt),
                "The resumed deadline cannot be earlier than the current deadline.");
        }

        EffectiveDeadlineAt = newEffectiveDeadlineAt;
        StatusId = TestAttemptPartStatusIds.InProgress;
    }
}
