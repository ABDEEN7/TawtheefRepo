using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamExemptionDecision), Schema = Schemas.Hr)]
public class ExamExemptionDecision : EventEntity
{
    public Guid InvitationId { get; set; }
    public required string Reason { get; set; }
    public Guid StatusId { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Invitation? Invitation { get; set; }
    public ExamExemptionDecisionStatus? Status { get; set; }
}
