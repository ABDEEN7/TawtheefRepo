namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

// CandidateQid is the candidate's personal ID (UserProfile.NationalNumber) - null when the profile has none.
public sealed record ScheduleSlotPreviewDto(
    GeneratedSlotDto Slot,
    Guid? InvitationId,
    string? CandidateFullNameAr,
    string? CandidateFullNameEn,
    string? CandidateQid);

// Server-computed preview of a wizard's current periods/distribution, used to render step 2/3
// live (capacity, candidate pairing) before the final atomic submit.
// EligibleMale/FemaleCount split EligibleCandidateCount by the profile's gender; a candidate whose profile has no
// gender counts toward the total only, so male + female can be lower than the total.
public sealed record SchedulePlanPreviewDto(
    int TotalCapacity,
    int EligibleCandidateCount,
    int EligibleMaleCount,
    int EligibleFemaleCount,
    int UnassignedCandidateCount,
    List<ScheduleSlotPreviewDto> Slots);
