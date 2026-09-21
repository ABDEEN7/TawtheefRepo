using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.TestSlots.DTOs;

public sealed record TestSlotListItemDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public Guid RoomId { get; init; }
    public required string RoomName { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int CandidateCount { get; init; }
    public Guid? HallSupervisorId { get; init; }
    public string? HallSupervisorName { get; init; }
    public bool IsCurrentUserAssigned { get; init; }
    public bool IsCurrentUserRoomHead { get; init; }
    public required DropdownOptions Status { get; init; }
}
