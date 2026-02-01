import { JobPointsDetailType } from "../types/job-points-detail.type";

export interface SaveJobPointsDetailDto {
  type: JobPointsDetailType;
  code: string;
  name: string;
  referenceId?: string | null;
  points: number;
}
