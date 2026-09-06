using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewSchedule), Schema = Schemas.Interview)]
public class InterviewSchedule : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid JobInterviewTemplateId { get; set; }
    public JobInterviewTemplate? JobInterviewTemplate { get; set; }

    public required string TitleAr { get; set; }
    public string? TitleEn { get; set; }

    public InterviewType DefaultInterviewType { get; set; }
    public int DefaultDurationMinutes { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Draft;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DecisionNotes { get; set; }

    public ICollection<InterviewAppointment> Appointments { get; set; } = [];
}

public enum ScheduleStatus
{
    Draft = 1,
    Proposed = 2,
    PendingApproval = 3,
    Returned = 4,
    Approved = 5,
    ReadyForExecution = 6,
    InProgress = 7,
    Closed = 8,
    Cancelled = 9
}

public enum InterviewType
{
    InPerson = 1,
    Remote = 2
}
