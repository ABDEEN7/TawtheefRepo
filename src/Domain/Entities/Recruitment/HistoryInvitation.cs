using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment;

public class HistoryInvitation : EventEntity
{
    public Guid InvitationId { get; set; }
    public Invitation? Invitation { get; set; }
    
    public Guid InvitationStatusId { get; set; }
    public InvitationStatus? InvitationStatus { get; set; }
    
    public string? Note { get; set; }
}
