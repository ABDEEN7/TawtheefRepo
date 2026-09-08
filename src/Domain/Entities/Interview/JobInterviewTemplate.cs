using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(JobInterviewTemplate), Schema = Schemas.Interview)]
public class JobInterviewTemplate : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid InterviewTemplateVersionId { get; set; }
    public InterviewTemplateVersion? InterviewTemplateVersion { get; set; }

    public JobInterviewTemplateStatus Status { get; set; } = JobInterviewTemplateStatus.Draft;

    public bool IsActive { get; set; } = true;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DecisionNotes { get; set; }
}

public enum JobInterviewTemplateStatus
{
    Draft = 1,
    PendingApproval = 2,
    Returned = 3,
    Approved = 4,
    Cancelled = 5
}
