using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class InvitationStatusIds
{
    //by sys
    public static readonly Guid NewInvitation = Guid.Parse("F0BC801D-F54C-4A0E-8AAE-00694E4FC80D");
    public static readonly Guid Closed = Guid.Parse("CA054592-8617-406D-8E8F-3A773B3D0D5E");
    //by employee
    public static readonly Guid Cancelled = Guid.Parse("BCED01D8-3784-4E2C-9312-930951CC59D8");
    public static readonly Guid ReturnedAttachment = Guid.Parse("9B86FA2C-D295-46D7-9092-23AD8AB7B10C");
    //by user
    public static readonly Guid Submitted = Guid.Parse("22EF7E86-28CB-4A30-98BC-7D45F9B44DE3");
    public static readonly Guid PendingAttachmentApproval = Guid.Parse("6608F560-4DC0-4F2A-A190-6743A9A8C5CB");
    public static readonly Guid Read = Guid.Parse("64236C6A-167A-4213-B1D6-80C2C8C86DDE");
    public static readonly Guid Rejected = Guid.Parse("7103BA49-AD43-4751-B1A5-9084ACA69676");
}
[Table(nameof(InvitationStatus), Schema = Schemas.Lookup)]
public class InvitationStatus : LookupBase
{
}
