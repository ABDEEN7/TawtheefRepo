using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewEvaluationAxis), Schema = Schemas.Interview)]
public class InterviewEvaluationAxis : EventEntity
{
    public required string NameAr { get; set; }

    public string? NameEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; } = true;
}
