using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class JobStatusIds
{
    public static readonly Guid Draft = Guid.Parse("C64916A6-4BD2-A4B6-AFBE-C5C3B4926530");
    public static readonly Guid Active = Guid.Parse("E0CD7B22-8948-0C37-9B15-2E5217F0C565");
    public static readonly Guid NeedUpdate = Guid.Parse("E0CD7B22-8948-0C37-9B15-2E5217F00065");
    public static readonly Guid Closed = Guid.Parse("114DAE76-BDE9-3EFA-2A2B-803EBD92E109");
    public static readonly Guid Cancelled = Guid.Parse("C0D95787-8505-B84F-6F50-0B461ECE500D");
    public static readonly Guid PendingApproval = Guid.Parse("E07BD71F-C466-0EEA-49CC-2B13D1D9403F");
    public static readonly Guid PendingPointConfiguration = Guid.Parse("5C360B07-157C-630A-254A-9C01587D80A8");
    public static readonly Guid NeedPointUpdate = Guid.Parse("3F2A1C9B-8B5D-4E1F-A3C9-1D7B4E5F6A2C");
    public static readonly Guid PendingPointApproval = Guid.Parse("2C3F9A4E-7D1B-4E9A-8C3D-1F2B3C4D5E6F");
    public static readonly Guid ReadyForAnnouncement = Guid.Parse("0D21E063-48D3-D320-4078-D85A7C2BF622");
    public static readonly Guid Published = Guid.Parse("1E3ECAD5-63FA-A11C-7ACB-DD4C62EF74FD");
    public static readonly Guid Rejected = Guid.Parse("F2E7748A-12FE-53AC-FBDF-E989F8AA498A");
}
[Table(nameof(JobStatus), Schema = Schemas.Lookup)]
public class JobStatus : LookupBase
{
}
