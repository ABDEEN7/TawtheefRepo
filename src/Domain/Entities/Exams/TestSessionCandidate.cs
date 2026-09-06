using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestSessionCandidate), Schema = Schemas.Hr)]
public class TestSessionCandidate : EventEntity
{
    public Guid TestSessionId { get; set; }
    public Guid InvitationId { get; set; }
    public Guid AttendanceStatusId { get; set; }
    public Guid IdentityVerificationStatusId { get; set; }
    public string? IdentityVerificationNotes { get; set; }
    public DateTime? AuthorizedAt { get; set; }
    public Guid StatusId { get; set; }
    public string? RescheduleReason { get; set; }

    public TestSession? TestSession { get; set; }
    public Invitation? Invitation { get; set; }
    public TestSessionCandidateAttendanceStatus? AttendanceStatus { get; set; }
    public TestSessionCandidateIdentityVerificationStatus? IdentityVerificationStatus { get; set; }
    public TestSessionCandidateStatus? Status { get; set; }
}
