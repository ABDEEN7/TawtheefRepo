using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

public class Invitation : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    
    public Guid ApplicantId { get; set; }
    public ApplicantUser? Applicant { get; set; }
    
    public bool IsAccepted { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    
    public Guid InvitationStatusId { get; set; }
    public InvitationStatus? InvitationStatus { get; set; }
    
    [NotMapped]
    public DateTimeOffset? InvitationAt => CreatedDate;
    
    public ICollection<HistoryInvitation> History { get; init; } = [];
}
