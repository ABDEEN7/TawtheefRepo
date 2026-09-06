using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewAppointmentNote), Schema = Schemas.Interview)]
public class InterviewAppointmentNote : EventEntity
{
    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    public Guid NoteTypeId { get; set; }
    public InterviewNoteType? NoteType { get; set; }

    // NULL for HR notes written outside the committee.
    public Guid? AuthorCommitteeMemberId { get; set; }
    public InterviewCommitteeMember? AuthorCommitteeMember { get; set; }

    public required string Body { get; set; }
    public bool IsVisibleToCommittee { get; set; } = true;
}
