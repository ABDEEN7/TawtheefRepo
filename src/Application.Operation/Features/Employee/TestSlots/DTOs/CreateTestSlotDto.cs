namespace Application.Operation.Features.Employee.TestSlots.DTOs;

public sealed class CreateTestSlotDto
{
    public required string TitleAr { get; init; }
    public string? TitleEn { get; init; }
    public Guid RoomId { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public required List<CreateTestSlotStaffDto> Staff { get; init; }
}

public sealed class CreateTestSlotStaffDto
{
    public Guid StaffUserId { get; init; }
    public Guid RoleId { get; init; }
    public bool IsActive { get; init; }
}

public sealed record CreatedTestSlotDto(Guid Id, string SlotNo);

public sealed record TestSlotStaffConflictDto(
    Guid StaffUserId,
    string StaffDisplayName,
    string SlotTitle,
    DateOnly SlotDate,
    TimeOnly StartTime,
    TimeOnly EndTime);
