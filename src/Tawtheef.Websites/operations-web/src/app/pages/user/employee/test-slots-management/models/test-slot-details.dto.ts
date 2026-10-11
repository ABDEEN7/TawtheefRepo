import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface TestSlotStaffDetailsDto {
  userId: string;
  name: string;
  roleName: string;
}
export interface TestSlotSessionDto {
  id: string;
  sessionNo: string;
  examNo: string;
  statusName: string;
  candidateCount: number;
}
export interface TestSlotDetailsDto {
  id: string;
  titleAr: string;
  titleEn?: string;
  roomName: string;
  roomCapacity: number;
  slotDate: string;
  startTime: string;
  endTime: string;
  status: dropdownOptionsModel;
  candidateCount: number;
  totalCandidates: number;
  presentCandidates: number;
  waitingCandidates: number;
  absentCandidates: number;
  staff: TestSlotStaffDetailsDto[];
  isCurrentUserAssigned: boolean;
  isCurrentUserRoomHead: boolean;
  hasRevealableAccessCode: boolean;
}

export interface TestSlotAccessCodeDto {
  accessCode: string;
}
export interface TestSlotCandidateListItemDto {
  candidateId: string;
  candidateName: string;
  qid?: string;
  examTitle: string;
  jobTitle?: string;
  sessionNo: string;
  examNo: string;
  attendanceStatus: string;
  attemptStatus: string;
  testAttemptId: string | null;
  attemptStatusBackendName?: string | null;
  resumeAuthorized: boolean;
  resumeAllowed: boolean;
  rescheduleAllowed: boolean;
  notificationStatus?: string;
  attendanceStatusBackendName: string;
  identityVerificationStatus: string;
  identityVerificationStatusBackendName: string;
  candidateStatus: string;
  candidateStatusBackendName: string;
  identityVerificationNotes: string | null;
  hasOpenInterruption: boolean;
  sessionStatusBackendName: string;
}
export interface VerifyTestSlotCandidateIdentityRequest {
  matched: boolean;
  notes: string | null;
}
export interface TestSlotCandidateFilters {
  pageNumber: number;
  pageSize: number;
  language: string;
  search?: string | null;
  sessionId?: string | null;
  attendanceStatusId?: string | null;
}


export interface CandidateExamRescheduleOptionDto {
  testSessionId: string;
  sessionNo: string;

  testSlotId: string;
  slotTitleAr: string;
  slotTitleEn: string | null;
  slotDate: string;

  startTime: string;
  endTime: string;

  roomId: string;
  roomNameAr: string;
  roomNameEn: string | null;

  capacity: number;
  assignedCount: number;
  availableCapacity: number;
}

export interface RescheduleInterruptedCandidateExamRequest {
  targetTestSessionId: string;
  reason: string;
}