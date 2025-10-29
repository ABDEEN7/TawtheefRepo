using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

public class Invitations : EventEntity
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
    
    public ICollection<HistoryInvitations> History { get; init; } = [];
}

public class HistoryInvitations : EventEntity
{
    public Guid InvitationId { get; set; }
    public Invitations? Invitation { get; set; }
    
    public Guid InvitationStatusId { get; set; }
    public InvitationStatus? InvitationStatus { get; set; }
    
    public string? Note { get; set; }
}
