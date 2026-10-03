import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { TestSessionCandidateListItemDto } from './test-session-candidate.dto';

export interface TestSessionListItemDto {
  id: string;
  sessionNo: string;
  examId: string;
  examName: string;
  jobId: string;
  jobTitle: string;
  sessionDate: string | null;
  periodId?: string | null;
  period: string | null;
  roomId: string | null;
  roomName: string | null;
  candidateCount: number;
  roomHeadName?: string;
  status: dropdownOptionsModel;
}

export const TEST_SESSION_STATUS_IDS = {
  draft: '11364bf4-c7bb-4326-a441-238864e53612',
  returned: '6c7b42ab-a356-47e6-93a1-a5e5a2dd09d4',
  pendingApproval: '16b3ac19-2ff1-491f-acca-7663c19c226b',
} as const;

export interface TestSessionEditDto {
  testSessionId: string;
  sessionNo: string;
  examId: string;
  statusId: string;
  genderFilter: 'Male' | 'Female' | null;
  nationalityFilter: 'Qatari' | 'NonQatari' | null;
  invitationIds: string[];
  persistedCandidates?: TestSessionCandidateListItemDto[];
  testSlotId: string | null;
  slotName: string | null;
  slotDate: string | null;
  slotStartTime: string | null;
  slotEndTime: string | null;
  roomId: string | null;
  roomName: string | null;
  roomCapacity: number | null;
  startTime: string | null;
  endTime: string | null;
  availableCapacity: number;
  decisionNote: string | null;
  decisionByName?: string | null;
  decisionAt?: string | null;
  statusBackendName?: string | null;
}

export interface TestSessionFilters {
  pageNumber: number;
  pageSize: number;
  language: string;
  searchText?: string;
  examId?: string;
  jobId?: string;
  roomId?: string;
  fromDate?: string;
  toDate?: string;
  periodId?: string;
  statusId?: string;
  nationalityId?: string;
  genderId?: string;
}

export interface TestSessionLookupsDto {
  exams: dropdownOptionsModel[];
  jobs: dropdownOptionsModel[];
  rooms: dropdownOptionsModel[];
  statuses: dropdownOptionsModel[];
  periods: dropdownOptionsModel[];
  nationalities: dropdownOptionsModel[];
  genders: dropdownOptionsModel[];
}
