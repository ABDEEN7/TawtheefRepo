import { JobPointsMain } from "./job-points-main.model";

export interface JobPointsConfig {
  jobId: string;
  totalMax: number;
  main: JobPointsMain;
  details: JobPointsMain[]; 
}