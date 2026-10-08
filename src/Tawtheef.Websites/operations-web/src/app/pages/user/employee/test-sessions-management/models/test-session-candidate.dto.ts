import { InvitationSource } from '../../../../../core/enums/invitation-source.enum';
import {
  TestSessionGenderFilter,
  TestSessionNationalityFilter,
} from './test-session-setup.dto';

export interface TestSessionCandidateListItemDto {
  invitationId: string;
  candidateNumber?: string | null;
  candidateName: string;
  jobTitle?: string | null;
  nationality?: string | null;
  gender?: string | null;
  highestQualification?: string | null;
  qualificationScore?: number | null;
  source: InvitationSource;
  eligibilityStatus: 'Eligible' | 'NotReady' | 'Excluded';
  eligibilityReason?: string | null;
}

export interface TestSessionCandidateSummaryDto {
  total: number;
  eligible: number;
  notReady: number;
  excluded: number;
}

export interface TestSessionCandidateConflictDto {
  invitationId: string;
  candidateName: string;
  qid?: string | null;
}

export interface TestSessionCandidatesDto {
  candidates: TestSessionCandidateListItemDto[];
  summary: TestSessionCandidateSummaryDto;
}

export interface TestSessionCandidateFilters {
  examId: string;
  language: string;
  genderFilter?: TestSessionGenderFilter;
  nationalityFilter?: TestSessionNationalityFilter;
  testSessionId?: string;
}
