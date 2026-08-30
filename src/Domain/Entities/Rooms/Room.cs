using Tawtheef.Domain.Common;
using Tawtheef.Domain.Enums.Rooms;

namespace Tawtheef.Domain.Entities.Rooms;

public sealed class Room : EventEntity
{
    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public RoomType RoomType { get; set; }

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public RoomStatus Status { get; set; }

    public string? Notes { get; set; }
}




