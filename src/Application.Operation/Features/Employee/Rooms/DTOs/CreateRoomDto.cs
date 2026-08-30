using Tawtheef.Domain.Enums.Rooms;

namespace Application.Operation.Features.Employee.Rooms.DTOs;

public sealed record CreateRoomDto(
    string NameAr,
    string NameEn,
    RoomType RoomType,
    int Capacity,
    string? Location,
    RoomStatus Status,
    string? Notes);
