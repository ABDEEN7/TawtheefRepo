import { Lookups } from "../../../core/models/lookups.model";
import { JobQuotasResponseDto } from "./job-quotas-response-Dto";

export interface JobResponseDto {
  id: string;

  createdById?: string;    
  createdDate: string;     
  updatedById?: string;
  updatedDate?: string;

  title: string;
  vacancies: number;
  deadline: string;
  description: string;
  benefits: string;
  publishAt?: string;

  requestingDepartment?: Lookups;
  jobCategory?: Lookups;
  gender?: Lookups;
  workLocation?: Lookups;
  major?: Lookups;
  workType?: Lookups;
  status?: Lookups;

  quota?: JobQuotasResponseDto;

  degrees: Lookups[];
  conditions: string[];
  skills: string[];
}
