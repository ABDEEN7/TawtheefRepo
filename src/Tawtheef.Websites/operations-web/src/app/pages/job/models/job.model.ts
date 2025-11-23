import { JobBasics } from './job-basics.models';
import { JobQuotas } from './job-quotas.models';
import { GUID } from '../../../shared/types/guid.type';

export interface Job {
  id?: GUID;
  basics: JobBasics;
  quotas: JobQuotas;
  description: string;
  benefits: string;
  conditions: string[];
  skills: string[];
  degreeIds: GUID[];
  status?: string;
  createdAt?: Date;
  updatedAt?: Date;
}
