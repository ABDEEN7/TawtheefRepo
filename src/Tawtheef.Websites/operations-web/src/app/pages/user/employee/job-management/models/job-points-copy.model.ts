import { JobPointsDetail } from './job-points-details.model';

export interface JobPointsCopy {
  applicantCategory: number;
  education: number;
  experience: number;
  training: number;
  skills: number;
  languages: number;
  certificates: number;
  total: number;
  details: JobPointsDetail[];
}