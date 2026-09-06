using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestSlotStaff), Schema = Schemas.Hr)]
public class TestSlotStaff : EventEntity
{
    public Guid TestSlotId { get; set; }
    public Guid StaffUserId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }

    public TestSlot? TestSlot { get; set; }
    public TestSlotStaffRole? Role { get; set; }
}
