import { AppointmentModel } from './appointment.model';
import { CommitteeRole, CommitteeStatus, InterviewType, ScheduleStatus } from './enums';

// Mirrors ScheduleListItemDto.
export interface ScheduleListItemModel {
  id: string;
  jobId: string;
  jobTitleId?: string | null;
  jobTitleNameAr?: string | null;
  jobTitleNameEn?: string | null;
  // The job's active committee (resolved server-side; the schedule itself has no committee FK).
  committeeNameAr?: string | null;
  committeeNameEn?: string | null;
  // The live slots folded into contiguous sittings, as UTC instants. The displayed days and daily hours are
  // derived from them in the viewer's timezone (summarizeSessions in schedule-time.ts).
  sessions: ScheduleSessionModel[];
  defaultInterviewType: InterviewType;
  candidatesCount: number;
  status: ScheduleStatus;
}

// Mirrors ScheduleSessionDto (UTC instants).
export interface ScheduleSessionModel {
  startAt: string;
  endAt: string;
}

// Mirrors SchedulePeriodDto - the period a saved schedule's live slots were generated from (reconstructed
// server-side; periods themselves are never stored). startAt/endAt are UTC instants.
export interface SchedulePeriodModel {
  startAt: string;
  endAt: string;
  roomId?: string | null;
  roomNameAr?: string | null;
  roomNameEn?: string | null;
  remoteMeetingUrl?: string | null;
  remoteMeetingInstructions?: string | null;
}

// Mirrors ScheduleDto.
export interface ScheduleModel {
  id: string;
  jobId: string;
  jobTitleNameAr?: string | null;
  jobTitleNameEn?: string | null;
  interviewTemplateId: string;
  interviewTemplateTitleAr: string;
  interviewTemplateTitleEn?: string | null;
  interviewCommitteeId?: string | null;
  committeeNameAr?: string | null;
  committeeNameEn?: string | null;
  titleAr: string;
  titleEn?: string | null;
  defaultInterviewType: InterviewType;
  defaultDurationMinutes: number;
  defaultBufferMinutes: number;
  status: ScheduleStatus;
  approvedById?: string | null;
  approvedAt?: string | null;
  decisionNotes?: string | null;
  appointments: AppointmentModel[];
  periods: SchedulePeriodModel[];
}

// Mirrors ScheduleCommitteeMemberDto.
export interface ScheduleCommitteeMemberModel {
  memberUserId: string;
  fullNameAr: string;
  fullNameEn?: string | null;
  role: CommitteeRole;
}

// Mirrors ScheduleCreationContextDto - feeds wizard steps 1 and 2.
export interface CreationContextModel {
  jobId: string;
  jobTitleNameAr: string;
  jobTitleNameEn?: string | null;
  majorNameAr?: string | null;
  majorNameEn?: string | null;
  departmentNameAr?: string | null;
  departmentNameEn?: string | null;
  interviewTemplateId: string;
  interviewTemplateTitleAr: string;
  interviewTemplateTitleEn?: string | null;
  interviewTemplateHasApprovedVersion: boolean;
  interviewCommitteeId: string;
  committeeNameAr: string;
  committeeNameEn?: string | null;
  committeeStatus: CommitteeStatus;
  members: ScheduleCommitteeMemberModel[];
  eligibleCandidateCount: number;
  eligibleMaleCount: number;
  eligibleFemaleCount: number;
}

// Mirrors RoomOptionDto - feeds the Step 2 In-Person room picker.
export interface RoomOptionModel {
  id: string;
  nameAr: string;
  nameEn?: string | null;
  locationNameAr: string;
  locationNameEn?: string | null;
  capacity: number;
}
