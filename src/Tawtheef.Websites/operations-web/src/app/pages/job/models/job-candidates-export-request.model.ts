import { GUID } from '../../../shared/types/guid.type';
import { JobCandidatesFilter } from './job-candidates-filter.model';

export interface JobCandidatesExportRequest {
  jobId: GUID;
  filter?: JobCandidatesFilter;
  invitationIds?: GUID[];
}
