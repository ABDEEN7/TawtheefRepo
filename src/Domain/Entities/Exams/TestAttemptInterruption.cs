using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestAttemptInterruption), Schema = Schemas.Hr)]
public class TestAttemptInterruption : EventEntity
{
    public Guid TestAttemptId { get; set; }
    public Guid? TestAttemptPartId { get; set; }
    public DateTime InterruptedAt { get; set; }
    public string? Reason { get; set; }
    public Guid StatusId { get; set; }
    public Guid? ResolutionActionId { get; set; }
    public string? ResolutionNotes { get; set; }
    public Guid? ResolvedById { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public TestAttempt? TestAttempt { get; set; }
    public TestAttemptPart? TestAttemptPart { get; set; }
    public TestAttemptInterruptionStatus? Status { get; set; }
    public TestAttemptInterruptionResolutionAction? ResolutionAction { get; set; }
}
