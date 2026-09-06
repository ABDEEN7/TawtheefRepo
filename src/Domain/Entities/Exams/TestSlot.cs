using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestSlot), Schema = Schemas.Hr)]
public class TestSlot : EventEntity
{
    public required string TitleAr { get; set; }
    public string? TitleEn { get; set; }
    public Guid RoomId { get; set; }
    public DateOnly SlotDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public required string AccessCodeHash { get; set; }
    public Guid StatusId { get; set; }
    public Guid? StartedById { get; set; }
    public DateTime? StartedAt { get; set; }

    public Room? Room { get; set; }
    public TestSlotStatus? Status { get; set; }
}
