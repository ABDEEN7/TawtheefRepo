using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Services;

// Double-booking checks are global, not scoped to one schedule - a committee or an
// InPerson room can only be in one active appointment at a time across the entire system.
public static class ScheduleConflictChecker
{
    // Mirrors IX_Appointment_Conflict's filter: everything except Closed/Rescheduled/Cancelled
    // still occupies its declared time and must be checked.
    private static readonly AppointmentStatus[] ActiveStatuses =
    [
        AppointmentStatus.Held, AppointmentStatus.Scheduled, AppointmentStatus.InInterview,
        AppointmentStatus.UnderEvaluation, AppointmentStatus.Completed
    ];

    public static async Task<Result> ValidateNoConflictsAsync(
        IUnitOfWork unitOfWork, Guid interviewCommitteeId, IReadOnlyList<GeneratedSlotDto> slots,
        CancellationToken cancellationToken, Guid? excludeAppointmentId = null, Guid? excludeScheduleId = null)
    {
        if (slots.Count == 0)
            return Result.Ok();

        var minStart = slots.Min(s => s.StartAt);
        var maxEnd = slots.Max(s => s.EndAt);
        var roomIds = slots.Where(s => s.RoomId is not null).Select(s => s.RoomId!.Value).Distinct().ToList();

        var existing = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Where(a => ActiveStatuses.Contains(a.Status) && a.StartAt < maxEnd && a.EndAt > minStart)
            .Where(a => excludeAppointmentId == null || a.Id != excludeAppointmentId)
            .Where(a => excludeScheduleId == null || a.InterviewScheduleId != excludeScheduleId)
            .Where(a => a.InterviewCommitteeId == interviewCommitteeId || (a.RoomId != null && roomIds.Contains(a.RoomId.Value)))
            .Select(a => new { a.InterviewCommitteeId, a.RoomId, a.StartAt, a.EndAt })
            .ToListAsync(cancellationToken);

        if (existing.Count == 0)
            return Result.Ok();

        foreach (var slot in slots)
        {
            foreach (var other in existing)
            {
                var overlaps = slot.StartAt < other.EndAt && other.StartAt < slot.EndAt;
                if (!overlaps)
                    continue;

                if (other.InterviewCommitteeId == interviewCommitteeId)
                    return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentCommitteeConflict));

                if (slot.RoomId is not null && other.RoomId == slot.RoomId)
                    return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentRoomConflict));
            }
        }

        return Result.Ok();
    }
}
