using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

public sealed record AppointmentDto(
    Guid Id,
    Guid? InvitationId,
    string? CandidateFullNameAr,
    string? CandidateFullNameEn,
    // The candidate's personal ID (UserProfile.NationalNumber); null for an open slot or a profile without one.
    string? CandidateQid,
    Guid InterviewCommitteeId,
    InterviewType InterviewType,
    Guid? RoomId,
    string? RoomNameAr,
    string? RoomNameEn,
    string? RemoteMeetingUrl,
    string? RemoteMeetingInstructions,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status,
    AttendanceStatus? AttendanceStatus,
    DateTimeOffset? ActualStartAt,
    DateTimeOffset? ActualEndAt,
    DateTimeOffset? ClosedAt,
    Guid? RescheduledFromAppointmentId,
    string? RescheduleReason,
    string? CancellationReason,
    DateTimeOffset? InvitationSentAt,
    DateTimeOffset? LastReminderSentAt,
    int ReminderCount);
