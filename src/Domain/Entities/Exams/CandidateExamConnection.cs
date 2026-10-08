using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(CandidateExamConnection), Schema = Schemas.Hr)]
public class CandidateExamConnection
{
    public string ConnectionId { get; set; } = null!;
    public Guid TestAttemptId { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public TestAttempt TestAttempt { get; set; } = null!;
}
