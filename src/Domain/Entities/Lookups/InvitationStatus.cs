using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class InvitationStatusIds
{
    public static readonly Guid NEW_INVITATION = Guid.Parse("F0BC801D-F54C-4A0E-8AAE-00694E4FC80D");
    public static readonly Guid CLOSED = Guid.Parse("CA054592-8617-406D-8E8F-3A773B3D0D5E");
    public static readonly Guid UNDER_REVIEW = Guid.Parse("43AD4950-46A9-4B37-B4E2-4A2D8AC4A7C4");
    public static readonly Guid APPROVED = Guid.Parse("22EF7E86-28CB-4A30-98BC-7D45F9B44DE3");
    public static readonly Guid READED = Guid.Parse("64236C6A-167A-4213-B1D6-80C2C8C86DDE");
    public static readonly Guid REJECTED = Guid.Parse("7103BA49-AD43-4751-B1A5-9084ACA69676");
    public static readonly Guid CANCELLED = Guid.Parse("BCED01D8-3784-4E2C-9312-930951CC59D8");
    public static readonly Guid REQUIRES_UPDATE = Guid.Parse("5602DBCE-00A3-488B-9156-981A0FB18A01");
    public static readonly Guid SUBMITTED = Guid.Parse("E7B28F10-FB23-466E-A9B2-AAA3C9AFDEAC");
}
[Table(nameof(InvitationStatus), Schema = Schemas.Lookup)]
public class InvitationStatus : LookupBase
{
}
