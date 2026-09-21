namespace Application.Operation.Features.Employee.TestSlots.DTOs;

public sealed class CreateTestSlotDto
{
    public required string TitleAr { get; init; }
    public string? TitleEn { get; init; }
    public Guid RoomId { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public List<TestSlotStaffAssignmentDto>? Staff { get; init; }
}

public sealed class TestSlotStaffAssignmentDto
{
    public Guid StaffUserId { get; init; }
    public Guid RoleId { get; init; }
    public bool IsActive { get; init; }
}

public sealed class UpdateTestSlotAssignmentsDto
{
    public List<TestSlotStaffAssignmentDto>? Staff { get; init; }
}

public sealed record SavedTestSlotDto(Guid Id);

public sealed class TestSlotConfigurationDto
{
    public Guid Id { get; init; }
    public required string TitleAr { get; init; }
    public string? TitleEn { get; init; }
    public Guid RoomId { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public required List<TestSlotConfigurationStaffDto> Staff { get; init; }
}

public sealed class TestSlotConfigurationStaffDto
{
    public Guid StaffUserId { get; init; }
    public required string Name { get; init; }
    public Guid RoleId { get; init; }
}

public sealed record TestSlotStaffConflictDto(
    Guid StaffUserId,
    string StaffDisplayName,
    string SlotTitle,
    DateOnly SlotDate,
    TimeOnly StartTime,
    TimeOnly EndTime);
