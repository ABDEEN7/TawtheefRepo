using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewTemplateEvaluationAxis), Schema = Schemas.Interview)]
[Index(nameof(InterviewTemplateVersionId), nameof(InterviewEvaluationAxisId), IsUnique = true)]
public class InterviewTemplateEvaluationAxis : EventEntity
{
    public Guid InterviewTemplateVersionId { get; set; }
    public InterviewTemplateVersion? InterviewTemplateVersion { get; set; }

    public Guid InterviewEvaluationAxisId { get; set; }
    public InterviewEvaluationAxis? InterviewEvaluationAxis { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal MaxScore { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? QualificationScore { get; set; }

    public int OrderNo { get; set; }

    public ICollection<InterviewTemplateEvaluationCriterion> Criteria { get; init; } = [];
}
