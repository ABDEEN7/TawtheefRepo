import {JobBasics} from './job-basics.models';
import {JobQuotas} from './job-quotas.models';
import {JobStatusEnum} from '../enums/job-status.enum';
import { GUID } from '../../../shared/types/guid.type';

export interface Job {
  id?: GUID;
  basics: JobBasics;
  quotas: JobQuotas;
  conditions: string[];
  skills: string[];
  description: string;
  benefits: string;
  status?: JobStatusEnum;
  createdAt?: Date;
  updatedAt?: Date;
}
