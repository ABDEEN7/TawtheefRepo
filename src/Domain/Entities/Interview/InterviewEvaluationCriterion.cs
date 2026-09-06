using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewEvaluationCriterion), Schema = Schemas.Interview)]
[Index(nameof(InterviewEvaluationAxisId))]
public class InterviewEvaluationCriterion : EventEntity
{
    public Guid InterviewEvaluationAxisId { get; set; }
    public InterviewEvaluationAxis? Axis { get; set; }

    public required string NameAr { get; set; }

    public string? NameEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; } = true;
}
