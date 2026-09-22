import { AppointmentStatus, InterviewType } from './enums';

// Mirrors Application.Operation.Features.Interview.Schedule.DTOs.AppointmentDto exactly.
export interface AppointmentModel {
  id: string;
  invitationId?: string | null;
  candidateFullNameAr?: string | null;
  candidateFullNameEn?: string | null;
  // The candidate's personal ID (QID) - null for an open slot or a profile without one.
  candidateQid?: string | null;
  interviewCommitteeId: string;
  interviewType: InterviewType;
  roomId?: string | null;
  roomNameAr?: string | null;
  roomNameEn?: string | null;
  remoteMeetingUrl?: string | null;
  remoteMeetingInstructions?: string | null;
  startAt: string;
  endAt: string;
  status: AppointmentStatus;
  attendanceStatus?: number | null;
  actualStartAt?: string | null;
  actualEndAt?: string | null;
  closedAt?: string | null;
  rescheduledFromAppointmentId?: string | null;
  rescheduleReason?: string | null;
  cancellationReason?: string | null;
  invitationSentAt?: string | null;
  lastReminderSentAt?: string | null;
  reminderCount: number;
}
