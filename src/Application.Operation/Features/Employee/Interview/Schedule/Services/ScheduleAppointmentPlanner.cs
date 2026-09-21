using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Services;

// Pure computation - no DB access - so Create and Update (resubmit) handlers, and the preview
// query, all share the exact same slot-generation and distribution rules : "never
// trust a client-submitted capacity/slot list - always recompute server-side").
public static class ScheduleAppointmentPlanner
{
    public static int ComputeCapacity(TimeOnly startTime, TimeOnly endTime, int durationMinutes, int bufferMinutes)
    {
        var perSlot = durationMinutes + bufferMinutes;
        if (perSlot <= 0 || endTime <= startTime)
            return 0;

        var totalMinutes = (endTime - startTime).TotalMinutes + bufferMinutes;
        return (int)Math.Floor(totalMinutes / perSlot);
    }

    public static Result<List<GeneratedSlotDto>> GenerateSlots(
        IReadOnlyList<PeriodInputDto> periods, InterviewType interviewType, int durationMinutes, int bufferMinutes)
    {
        if (periods.Count == 0)
            return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewScheduleNoPeriods));

        if (durationMinutes < ScheduleConstants.MinimumDurationMinutes)
            return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewScheduleDurationTooShort));

        if (bufferMinutes < ScheduleConstants.MinimumBufferMinutes)
            return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewScheduleBufferInvalid));

        var ranges = new List<(DateOnly Date, DateTime Start, DateTime End, PeriodInputDto Period)>();

        foreach (var period in periods)
        {
            if (period.EndTime <= period.StartTime)
                return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewSchedulePeriodInvalidTime));

            var hasLocation = interviewType == InterviewType.InPerson
                ? period.RoomId is not null
                : !string.IsNullOrWhiteSpace(period.RemoteMeetingUrl);
            if (!hasLocation)
                return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewSchedulePeriodMissingLocation));

            if (ComputeCapacity(period.StartTime, period.EndTime, durationMinutes, bufferMinutes) <= 0)
                return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewSchedulePeriodZeroCapacity));

            ranges.Add((period.Date, period.Date.ToDateTime(period.StartTime), period.Date.ToDateTime(period.EndTime), period));
        }

        // One committee runs this whole schedule, so it can only be in one place at a time -
        // no two periods in the same draft may overlap, regardless of room.
        for (var i = 0; i < ranges.Count; i++)
            for (var j = i + 1; j < ranges.Count; j++)
                if (ranges[i].Date == ranges[j].Date && ranges[i].Start < ranges[j].End && ranges[j].Start < ranges[i].End)
                    return Result.Fail<List<GeneratedSlotDto>>(new Error(ErrorsCodes.InterviewSchedulePeriodOverlapsSelf));

        var slots = new List<GeneratedSlotDto>();
        foreach (var (date, start, end, period) in ranges)
        {
            var cursor = start;
            while (cursor.AddMinutes(durationMinutes) <= end)
            {
                slots.Add(new GeneratedSlotDto(
                    date, cursor, cursor.AddMinutes(durationMinutes),
                    period.RoomId, period.RemoteMeetingUrl, period.RemoteMeetingInstructions));
                cursor = cursor.AddMinutes(durationMinutes + bufferMinutes);
            }
        }

        return Result.Ok(slots.OrderBy(s => s.StartAt).ToList());
    }

    // Walks the sorted slot list and the eligible-candidate list in parallel, pairing them 1:1.
    // Manual overrides are applied first; remaining slots/candidates auto-fill in order. Excess
    // slots (capacity > candidates) are returned unpaired (InvitationId = null) - they still get
    // persisted as open "Held" appointments so a later-eligible candidate can be assigned to one
    // without regenerating the whole schedule.
    public static Result<List<(GeneratedSlotDto Slot, Guid? InvitationId)>> DistributeCandidates(
        IReadOnlyList<GeneratedSlotDto> slots,
        IReadOnlyList<Guid> eligibleInvitationIdsInOrder,
        IReadOnlyList<SlotAssignmentDto>? manualAssignments)
    {
        if (slots.Count < eligibleInvitationIdsInOrder.Count)
            return Result.Fail<List<(GeneratedSlotDto, Guid?)>>(new Error(ErrorsCodes.InterviewScheduleInsufficientCapacity));

        var eligibleSet = eligibleInvitationIdsInOrder.ToHashSet();
        var slotAssignment = new Dictionary<DateTime, Guid>();
        var usedCandidates = new HashSet<Guid>();

        if (manualAssignments is { Count: > 0 })
        {
            foreach (var manual in manualAssignments)
            {
                if (slots.All(s => s.StartAt != manual.SlotStartAt))
                    return Result.Fail<List<(GeneratedSlotDto, Guid?)>>(new Error(ErrorsCodes.InterviewScheduleSlotNotFound));

                if (!eligibleSet.Contains(manual.InvitationId))
                    return Result.Fail<List<(GeneratedSlotDto, Guid?)>>(new Error(ErrorsCodes.InterviewScheduleCandidateNotEligible));

                if (!slotAssignment.TryAdd(manual.SlotStartAt, manual.InvitationId))
                    return Result.Fail<List<(GeneratedSlotDto, Guid?)>>(new Error(ErrorsCodes.InterviewScheduleSlotAlreadyAssigned));

                if (!usedCandidates.Add(manual.InvitationId))
                    return Result.Fail<List<(GeneratedSlotDto, Guid?)>>(new Error(ErrorsCodes.InterviewScheduleCandidateAlreadyAssigned));
            }
        }

        var remainingCandidates = new Queue<Guid>(eligibleInvitationIdsInOrder.Where(id => !usedCandidates.Contains(id)));

        foreach (var slot in slots)
        {
            if (remainingCandidates.Count == 0)
                break;
            if (slotAssignment.ContainsKey(slot.StartAt))
                continue;

            slotAssignment[slot.StartAt] = remainingCandidates.Dequeue();
        }

        if (remainingCandidates.Count > 0)
            return Result.Fail<List<(GeneratedSlotDto, Guid?)>>(new Error(ErrorsCodes.InterviewScheduleCandidateUnassigned));

        var result = slots
            .Select(s => (Slot: s, InvitationId: slotAssignment.TryGetValue(s.StartAt, out var invitationId) ? invitationId : (Guid?)null))
            .ToList();

        return Result.Ok(result);
    }
}
