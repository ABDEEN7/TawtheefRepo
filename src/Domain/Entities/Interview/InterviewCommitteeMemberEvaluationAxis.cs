using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewCommitteeMemberEvaluationAxis), Schema = Schemas.Interview)]
[Index(nameof(InterviewCommitteeMemberId), nameof(InterviewTemplateEvaluationAxisId), IsUnique = true)]
public class InterviewCommitteeMemberEvaluationAxis : EventEntity
{
    public Guid InterviewCommitteeMemberId { get; set; }
    public InterviewCommitteeMember? InterviewCommitteeMember { get; set; }

    public Guid InterviewTemplateEvaluationAxisId { get; set; }
    public InterviewTemplateEvaluationAxis? InterviewTemplateEvaluationAxis { get; set; }
}
