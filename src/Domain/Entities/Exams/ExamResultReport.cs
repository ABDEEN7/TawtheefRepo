using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamResultReport), Schema = Schemas.Hr)]
public class ExamResultReport : EventEntity
{
    public Guid ExamId { get; set; }
    public decimal AppliedQualificationScore { get; set; }
    public Guid StatusId { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DecisionNotes { get; set; }

    public Exam? Exam { get; set; }
    public ExamResultReportStatus? Status { get; set; }
}
