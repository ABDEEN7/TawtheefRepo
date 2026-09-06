using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(Room), Schema = Schemas.Hr)]
public class Room : EventEntity
{
    public required string NameAr { get; set; }
    public string? NameEn { get; set; }
    public Guid LocationId { get; set; }
    public Guid RoomTypeId { get; set; }
    public int Capacity { get; set; }
    public Guid StatusId { get; set; }
    public string? Notes { get; set; }

    public Location? Location { get; set; }
    public RoomType? RoomType { get; set; }
    public RoomStatus? Status { get; set; }
}
