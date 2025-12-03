import { JobQuota } from './job-quotas.models';
import { GUID } from '../../../shared/types/guid.type';
import { JobDegree } from './job-degree.model';
import { JobCondition } from './job-condition.model';
import { JobSkill } from './job-skill.model';
import { JobResponsibility } from './job-responsibility.model';
import { RequiredAttachment } from './required-attachment.model';

export interface Job {
  id?: GUID;
  
  // Basic Information
  title: string;
  vacancies: number;
  deadline: Date | null;
  description: string;
  benefits: string;
  overview: string;
  qualificationsDescription?: string;
  publishAt?: Date | null;
  
  // Requirements
  minimumExperienceYears: number;
  minimumAge: number;
  maximumAge: number;
  
  // Foreign keys
  requestingDepartmentId: GUID;
  sectorId: GUID;
  managementId: GUID;
  jobCategoryId: GUID;
  genderId?: GUID;
  workLocationId: GUID;
  majorId: GUID;
  subMajorId?: GUID;
  workTypeId: GUID;
  statusId: GUID | null;
  
  // Quota
  quota: JobQuota;
  
  // Collections
  degrees: JobDegree[];
  conditions: JobCondition[];
  skills: JobSkill[];
  responsibilities: JobResponsibility[];
  requiredAttachments: RequiredAttachment[];
  
  // Metadata
  status?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

