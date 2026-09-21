namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

public sealed record RoomOptionDto(
    Guid Id,
    string NameAr,
    string? NameEn,
    string LocationNameAr,
    string? LocationNameEn,
    int Capacity);
