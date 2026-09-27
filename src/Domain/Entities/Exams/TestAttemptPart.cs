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

    public TestAttempt? TestAttempt { get; set; }
    public ExamPart? ExamPart { get; set; }
    public TestAttemptPartStatus? Status { get; set; }
    public ICollection<TestAttemptQuestion> Questions { get; set; } = new List<TestAttemptQuestion>();
}
