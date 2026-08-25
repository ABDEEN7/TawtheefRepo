using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Invitation), Schema = Schemas.Hr)]
[Index(nameof(JobId))]
[Index(nameof(ApplicantId))]
[Index(nameof(InvitationStatusId))]
public class Invitation : EventEntity
{
    public Guid JobId { get; init; }
    public Job? Job { get; init; }
    
    public Guid ApplicantId { get; init; }
    public ApplicantUser? Applicant { get; init; }
    
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    
    public Guid InvitationStatusId { get; set; }
    public InvitationStatus? InvitationStatus { get; init; }

    public InvitationSource Source { get; init; } = InvitationSource.Normal;

    public Guid BatchNumber { get; init; }
    public DateOnly ExpiresOn  { get; init; }
    
    [NotMapped]
    public DateTime? InvitedAt => CreatedDate;
    
    public ICollection<HistoryInvitation> History { get; init; } = [];
    public ICollection<InvitationAttachment> Attachments { get; init; } = [];

    public bool CanModifyAttachments =>
        InvitationStatusId == InvitationStatusIds.NewInvitation ||
        InvitationStatusId == InvitationStatusIds.Read ||
        InvitationStatusId == InvitationStatusIds.ReturnedAttachment;

    public void ChangeInvitationStatus(Guid newInvitationStatusId) 
    {
        this.InvitationStatusId = newInvitationStatusId;
    }

    public void CheckIfExpired(DateOnly currentDate)
    {
        var canExpire = InvitationStatusId == InvitationStatusIds.NewInvitation ||
                        InvitationStatusId == InvitationStatusIds.Read;

        if(canExpire && ExpiresOn < currentDate)
        {
            InvitationStatusId = InvitationStatusIds.Expired;
        }
    }
}
