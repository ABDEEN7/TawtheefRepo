using Tawtheef.Domain.Enums.Rooms;

namespace Application.Operation.Features.Employee.Rooms.DTOs;

public sealed record RoomDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public string? Location { get; init; }
    public RoomType RoomType { get; init; }
    public int Capacity { get; init; }
    public RoomStatus Status { get; init; }
    public string? Notes { get; init; }
    public DateTime LastUpdated { get; init; }
}
