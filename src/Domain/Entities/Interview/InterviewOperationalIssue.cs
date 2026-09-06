using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewOperationalIssue), Schema = Schemas.Interview)]
public class InterviewOperationalIssue : EventEntity
{
    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    public OperationalIssueType IssueType { get; set; }
    public string? Description { get; set; }
    public bool IsBlocking { get; set; }
    public OperationalIssueStatus Status { get; set; } = OperationalIssueStatus.Open;

    public Guid? ResolvedById { get; set; }
    public User? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNotes { get; set; }
}

public enum OperationalIssueType
{
    NoShow = 1,
    CandidateWithdrawal = 2,
    IncompleteEvaluation = 3,
    TechnicalProblem = 4,
    CouldNotBeConducted = 5,
    RescheduleRequest = 6
}

public enum OperationalIssueStatus
{
    Open = 1,
    Resolved = 2,
    Waived = 3
}
