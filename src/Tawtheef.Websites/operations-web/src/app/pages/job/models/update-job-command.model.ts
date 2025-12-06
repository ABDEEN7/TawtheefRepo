import { GUID } from '../../../shared/types/guid.type';
import { UpdateJobRequest } from './update-job-request.model';

export interface UpdateJobCommand {
  jobId: GUID;
  job: UpdateJobRequest;
}