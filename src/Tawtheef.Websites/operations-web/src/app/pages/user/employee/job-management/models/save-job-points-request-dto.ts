import { SaveJobPointsDetailDto } from "./save-job-points-detail-dto";

export interface SaveJobPointsRequestDto {
  jobId: string;

  applicantCategory: number;
  education: number;
  experience: number;
  training: number;
  skills: number;
  languages: number;
  certificates: number;
  total: number;

  details: SaveJobPointsDetailDto[];
}