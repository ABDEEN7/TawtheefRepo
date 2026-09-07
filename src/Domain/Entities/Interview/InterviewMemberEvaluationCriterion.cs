using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewMemberEvaluationCriterion), Schema = Schemas.Interview)]
[Index(nameof(InterviewMemberEvaluationId), nameof(InterviewTemplateEvaluationCriterionId), IsUnique = true)]
public class InterviewMemberEvaluationCriterion : EventEntity
{
    public Guid InterviewMemberEvaluationId { get; set; }
    public InterviewMemberEvaluation? InterviewMemberEvaluation { get; set; }

    public Guid InterviewTemplateEvaluationCriterionId { get; set; }
    public InterviewTemplateEvaluationCriterion? InterviewTemplateEvaluationCriterion { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal Score { get; set; }

    public string? Notes { get; set; }
}
