namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record ReadyTestSlotListItemDto
{
    public Guid TestSlotId { get; init; }
    public required string SlotName { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public Guid RoomId { get; init; }
    public required string RoomName { get; init; }
    public int RoomCapacity { get; init; }
    public int CurrentReservations { get; init; }
    public int ExistingSessionCount { get; init; }
    public int ExistingExamSessionCount { get; init; }
    public int RemainingCapacity { get; init; }
    public required string Status { get; init; }
}
