using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;

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

    // Guarded by the parent version's editability rather than its own state - an axis can only
    // change while the version that owns it is still Draft or Returned.
    public Result UpdateScore(decimal maxScore, decimal? qualificationScore, int orderNo)
    {
        var editable = InterviewTemplateVersion!.EnsureEditable();
        if (editable.IsFailed)
            return editable;

        MaxScore = maxScore;
        QualificationScore = qualificationScore;
        OrderNo = orderNo;
        return Result.Ok();
    }

    // A criterion either comes from the evaluation bank (no name override) or is a one-off custom
    // criterion for this template version only (must carry its own NameAr) - never both, never neither.
    public Result<InterviewTemplateEvaluationCriterion> AddCriterion(
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
        var editable = InterviewTemplateVersion!.EnsureEditable();
        if (editable.IsFailed)
            return Result.Fail<InterviewTemplateEvaluationCriterion>(editable.Errors);

        if (!IsValidCriterionSource(interviewEvaluationCriterionId, nameAr, nameEn, descriptionAr, descriptionEn))
            return Result.Fail<InterviewTemplateEvaluationCriterion>(new Error(ErrorsCodes.InterviewTemplateVersionCriterionSourceConflict));

        var criterion = new InterviewTemplateEvaluationCriterion
        {
            InterviewTemplateEvaluationAxisId = Id,
            InterviewEvaluationCriterionId = interviewEvaluationCriterionId,
            NameAr = interviewEvaluationCriterionId.HasValue ? null : nameAr,
            NameEn = interviewEvaluationCriterionId.HasValue ? null : nameEn,
            DescriptionAr = interviewEvaluationCriterionId.HasValue ? null : descriptionAr,
            DescriptionEn = interviewEvaluationCriterionId.HasValue ? null : descriptionEn,
            MaxScore = maxScore,
            IsRequired = isRequired,
            OrderNo = orderNo,
            Notes = notes
        };

        Criteria.Add(criterion);
        return Result.Ok(criterion);
    }

    internal static bool IsValidCriterionSource(
        Guid? interviewEvaluationCriterionId, string? nameAr, string? nameEn, string? descriptionAr, string? descriptionEn)
    {
        if (interviewEvaluationCriterionId.HasValue)
            return string.IsNullOrWhiteSpace(nameAr) && string.IsNullOrWhiteSpace(nameEn)
                   && string.IsNullOrWhiteSpace(descriptionAr) && string.IsNullOrWhiteSpace(descriptionEn);

        return !string.IsNullOrWhiteSpace(nameAr);
    }
}
