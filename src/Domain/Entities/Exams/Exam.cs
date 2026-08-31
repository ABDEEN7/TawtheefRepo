using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(Exam), Schema = Schemas.Hr)]
public class Exam : EventEntity
{
    public Guid JobId { get; set; }
    public required string TitleAr { get; set; }
    public string? TitleEn { get; set; }
    public int TotalQuestions { get; set; }
    public bool AllowPreviousQuestion { get; set; }
    public Guid InterruptionPolicyId { get; set; }
    public Guid StatusId { get; set; }
    public string? Notes { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DecisionNotes { get; set; }

    public Job? Job { get; set; }
    public ExamInterruptionPolicy? InterruptionPolicy { get; set; }
    public ExamStatus? Status { get; set; }
}
