namespace Application.Operation.Features.Employee.Rooms.DTOs;

public sealed record CreateRoomDto(
    string NameAr,
    string NameEn,
    Guid RoomTypeId,
    int Capacity,
    Guid LocationId,
    Guid StatusId,
    string? Notes);
