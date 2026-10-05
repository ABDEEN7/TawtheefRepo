namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// One open slot a candidate can be rescheduled into. Every slot seats exactly one candidate (see
// InterviewAppointment.IsOpenSlot), so each listed slot is one remaining seat - the dialog sums them
// per day for the "N available" capacity. StartAt/EndAt are UTC instants exposed via AsUtcOffset().
public sealed record RescheduleSlotDto(
    Guid SlotId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    Guid? RoomId,
    string? RoomNameAr,
    string? RoomNameEn,
    string? RemoteMeetingUrl);
