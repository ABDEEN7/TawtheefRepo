using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;

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

    // Guarded by the parent version's editability (reached through the owning axis) rather than its own state.
    public Result Update(
        Guid? interviewEvaluationCriterionId,
        string? nameAr,
        string? nameEn,
        string? descriptionAr,
        string? descriptionEn,
        decimal maxScore,
        bool isRequired,
        int orderNo,
        string? notes)
    {
        var editable = InterviewTemplateEvaluationAxis!.InterviewTemplateVersion!.EnsureEditable();
        if (editable.IsFailed)
            return editable;

        if (!InterviewTemplateEvaluationAxis.IsValidCriterionSource(interviewEvaluationCriterionId, nameAr, nameEn, descriptionAr, descriptionEn))
            return Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionCriterionSourceConflict));

        InterviewEvaluationCriterionId = interviewEvaluationCriterionId;
        NameAr = interviewEvaluationCriterionId.HasValue ? null : nameAr;
        NameEn = interviewEvaluationCriterionId.HasValue ? null : nameEn;
        DescriptionAr = interviewEvaluationCriterionId.HasValue ? null : descriptionAr;
        DescriptionEn = interviewEvaluationCriterionId.HasValue ? null : descriptionEn;
        MaxScore = maxScore;
        IsRequired = isRequired;
        OrderNo = orderNo;
        Notes = notes;
        return Result.Ok();
    }
}
