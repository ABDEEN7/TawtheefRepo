namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

public sealed record ScheduleSlotPreviewDto(
    GeneratedSlotDto Slot,
    Guid? InvitationId,
    string? CandidateFullNameAr,
    string? CandidateFullNameEn);

// Server-computed preview of a wizard's current periods/distribution, used to render step 2/3
// live (capacity, candidate pairing) before the final atomic submit.
public sealed record SchedulePlanPreviewDto(
    int TotalCapacity,
    int EligibleCandidateCount,
    int UnassignedCandidateCount,
    List<ScheduleSlotPreviewDto> Slots);
