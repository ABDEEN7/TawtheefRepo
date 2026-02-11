using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Invitation), Schema = Schemas.Hr)]
public class Invitation : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    
    public Guid ApplicantId { get; set; }
    public ApplicantUser? Applicant { get; set; }
    
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    
    public Guid InvitationStatusId { get; set; }
    public InvitationStatus? InvitationStatus { get; set; }

    public Guid BatchNumber { get; set; }
    
    [NotMapped]
    public DateTime? InvitationAt => CreatedDate;
    
    public ICollection<HistoryInvitation> History { get; init; } = [];

    public void ChangeInvitationStatus(Guid newInvitationStatusId) 
    {
        this.InvitationStatusId = newInvitationStatusId;
    }
}
