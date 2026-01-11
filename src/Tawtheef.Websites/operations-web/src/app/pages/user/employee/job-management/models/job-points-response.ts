import { GUID } from '../../../../../shared/types/guid.type';
import { JobPointsDetailResponse } from './job-points-detail-response';

export interface JobPointsResponse {
  id: GUID;
  jobId: GUID;

  applicantCategory: number;
  education: number;
  experience: number;
  training: number;
  certificates: number;
  skills: number;
  languages: number;
  total: number;
  isApproved:boolean;
  details: JobPointsDetailResponse[];
}
