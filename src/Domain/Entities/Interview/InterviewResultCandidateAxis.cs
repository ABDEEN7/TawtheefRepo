using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

// A per-axis breakdown row of a frozen InterviewResultCandidate. Same "no true
// owned type" deviation as InterviewMemberEvaluationCriterion and
// InterviewTemplateEvaluationAxis -- a plain EventEntity with an FK, replaced as a
// set by the calculation service rather than an EF owned type.
[Table(nameof(InterviewResultCandidateAxis), Schema = Schemas.Interview)]
[Index(nameof(InterviewResultCandidateId), nameof(InterviewTemplateEvaluationAxisId), IsUnique = true)]
public class InterviewResultCandidateAxis : EventEntity
{
    public Guid InterviewResultCandidateId { get; set; }
    public InterviewResultCandidate? InterviewResultCandidate { get; set; }

    public Guid InterviewTemplateEvaluationAxisId { get; set; }
    public InterviewTemplateEvaluationAxis? InterviewTemplateEvaluationAxis { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal Score { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? QualificationScore { get; set; }

    public bool? QualificationMet { get; set; }
}
