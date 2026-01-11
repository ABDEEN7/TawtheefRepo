import { GUID } from '../../../../../shared/types/guid.type';
import { JobCandidatesFilter } from './job-candidates-filter.model';

export interface SendJobCandidateInvitationsRequest {
  jobId: GUID;
  filter?: JobCandidatesFilter;
  applicantIds  ?: GUID[];
}

export interface SendJobCandidateInvitationsResult {
  totalTargets: number;
  sentEmailCount: number;
  sentSmsCount: number;
  updatedStatusCount: number;
}
