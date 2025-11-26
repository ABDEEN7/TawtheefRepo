import { JobQuota } from './job-quotas.models';
import { GUID } from '../../../shared/types/guid.type';

export interface Job {
  id?: GUID;
  requestingDepartmentId:GUID;
  title: string;
  jobCategoryId: GUID;
  genderId: GUID;
  workLocationId: GUID;
  majorId: GUID;
  workTypeId: GUID;
  statusId :GUID;
  vacancies: number;
  deadline: Date | null;
  quota: JobQuota;
  description: string;
  benefits: string;
  conditions: string[];
  skills: string[];
  degreeIds: GUID[];
  status?: string;
  createdAt?: Date;
  updatedAt?: Date;
}
