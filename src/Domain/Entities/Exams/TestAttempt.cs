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
    public TestSessionCandidate? TestSessionCandidate { get; set; }
    public TestAttemptStatus? Status { get; set; }
    public ICollection<TestAttemptPart> Parts { get; set; } = new List<TestAttemptPart>();
    public ICollection<TestAttemptQuestion> Questions { get; set; } = new List<TestAttemptQuestion>();
    public ICollection<CandidateExamConnection> Connections { get; set; } = new List<CandidateExamConnection>();
}
