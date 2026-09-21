using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.DTOs;

public sealed record AppointmentDto(
    Guid Id,
    Guid? InvitationId,
    string? CandidateFullNameAr,
    string? CandidateFullNameEn,
    Guid InterviewCommitteeId,
    InterviewType InterviewType,
    Guid? RoomId,
    string? RemoteMeetingUrl,
    string? RemoteMeetingInstructions,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status,
    AttendanceStatus? AttendanceStatus,
    DateTime? ActualStartAt,
    DateTime? ActualEndAt,
    DateTime? ClosedAt,
    Guid? RescheduledFromAppointmentId,
    string? RescheduleReason,
    string? CancellationReason,
    DateTime? InvitationSentAt,
    DateTime? LastReminderSentAt,
    int ReminderCount);
