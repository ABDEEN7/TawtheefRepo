using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSessionStatusIds
{
    public static readonly Guid Draft = Guid.Parse("11364bf4-c7bb-4326-a441-238864e53612");
    public static readonly Guid Approved = Guid.Parse("a60f9202-8ab6-4c72-a5a4-43de299b46d6");
    public static readonly Guid Ready = Guid.Parse("3e3d2acc-4a2b-476c-acc1-eac2e9bb5380");
    public static readonly Guid InProgress = Guid.Parse("16b3ac19-2ff1-491f-acca-7663c19c226b");
    public static readonly Guid Closed = Guid.Parse("82b9fbdd-c9ec-46b5-a27b-001b47b22991");
    public static readonly Guid Cancelled = Guid.Parse("e543df74-3311-441e-aa14-928a918cf952");
}

[Table(nameof(TestSessionStatus), Schema = Schemas.Lookup)]
public class TestSessionStatus : LookupBase;

