import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface TestSlotStaffDetailsDto { userId: string; name: string; roleName: string; }
export type TestSessionGenderFilter = 'Male' | 'Female' | null;
export type TestSessionNationalityFilter = 'Qatari' | 'NonQatari' | null;
export interface TestSlotSessionDto {
  id: string;
  sessionNo: string;
  jobName: string;
  examName: string;
  examCode: string;
  candidateCount: number;
  startTime: string | null;
  endTime: string | null;
  genderFilter: TestSessionGenderFilter;
  nationalityFilter: TestSessionNationalityFilter;
  statusName: string;
}
export interface TestSlotDetailsDto {
  id: string; titleAr: string; titleEn?: string; roomName: string; roomCapacity: number;
  slotDate: string; startTime: string; endTime: string; status: dropdownOptionsModel; candidateCount: number;
  totalCandidates: number; presentCandidates: number; waitingCandidates: number; absentCandidates: number;
  staff: TestSlotStaffDetailsDto[];
  isCurrentUserAssigned: boolean; isCurrentUserRoomHead: boolean; hasRevealableAccessCode: boolean;
}

export interface TestSlotAccessCodeDto { accessCode: string; }
export interface TestSlotCandidateNotificationResultDto { notifiedCandidateCount: number; }
export interface TestSlotCandidateListItemDto {
  candidateId: string; candidateName: string; qid?: string; examTitle: string; jobTitle?: string;
  sessionNo: string; examNo: string; attendanceStatus: string; attemptStatus: string; notificationStatus?: string;
}
export interface TestSlotCandidateFilters {
  pageNumber: number; pageSize: number; language: string; search?: string | null; sessionId?: string | null;
  attendanceStatusId?: string | null;
}
