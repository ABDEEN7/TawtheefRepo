using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSlotStaffRoleIds
{
    public static readonly Guid HallSupervisor = Guid.Parse("f8535d6c-a02e-46d8-a124-db986e51d5fc");
    public static readonly Guid Monitor = Guid.Parse("7020e361-a374-4ebd-ab69-dd95d1de8789");
    public static readonly Guid Support = Guid.Parse("493cd1f0-2573-4140-a5e3-84c239b6a0c4");
}

[Table(nameof(TestSlotStaffRole), Schema = Schemas.Lookup)]
public class TestSlotStaffRole : LookupBase;

