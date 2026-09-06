using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamPart), Schema = Schemas.Hr)]
public class ExamPart : EventEntity
{
    public Guid ExamId { get; set; }
    public int PartNo { get; set; }
    public required string TitleAr { get; set; }
    public string? TitleEn { get; set; }
    public int DurationMinutes { get; set; }
    public decimal? QualificationScore { get; set; }

    public Exam? Exam { get; set; }
}
