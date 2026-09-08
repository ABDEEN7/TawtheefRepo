using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

// A criterion inside a version axis
/// <summary>
/// Represents an evaluation criterion within an interview template, associated with a specific evaluation axis. 
/// This entity is used to define the criteria against which candidates are evaluated during the interview process.
/// </summary>

[Table(nameof(InterviewTemplateEvaluationCriterion), Schema = Schemas.Interview)]
public class InterviewTemplateEvaluationCriterion : EventEntity
{
    public Guid InterviewTemplateEvaluationAxisId { get; set; }
    public InterviewTemplateEvaluationAxis? InterviewTemplateEvaluationAxis { get; set; }

    // we can add criterion for particular template without select criteron for criterion Bank -- so this criterion linked to this template version and axis in this InterviewTemplateEvaluationCriterion
    public Guid? InterviewEvaluationCriterionId { get; set; }                       
    public InterviewEvaluationCriterion? InterviewEvaluationCriterion { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal MaxScore { get; set; }

    public bool IsRequired { get; set; }
    public int OrderNo { get; set; }
    public string? Notes { get; set; }
}
