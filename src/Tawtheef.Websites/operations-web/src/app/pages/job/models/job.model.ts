import { GUID } from '../../../shared/types/guid.type';
import { JobDegree } from './job-degree.model';
import { JobCondition } from './job-condition.model';
import { JobSkill } from './job-skill.model';
import { JobResponsibility } from './job-responsibility.model';
import { JobRequiredAttachment } from './required-attachment.model';
import { JobTabReviewNote } from './job-tab-review-note';
import { JobReviewAttachment } from './job-review-attachment';
import { dropdownOptionsModel } from '../../../shared/models/dropdown-options.model';
import { JobTabReviewNoteResponse } from './job-tab-review-note-response';

export interface Job {
  titleAr: string;
  titleEn: string;
  sectorId: GUID;
  managementId: GUID;
  departmentId: GUID;
  yearsOfExperience: number;
  jobCategoryId: GUID;
  workLocationId: GUID;
  genderId?: GUID | null;          
  majorId: GUID;
  subMajorId?: GUID | null;         
  workTypeId: GUID;
  numberOfVacancies: number;
  closingDate: Date;              
  minimumAge: number;
  maximumAge: number;
  overviewAr?: string;
  overviewEn?: string;
  benefitsAr?: string;
  benefitsEn?: string;
  qualificationsDescriptionAr?: string;  
  qualificationsDescriptionEn?: string;  
  jobStatus?:dropdownOptionsModel;
  degrees?: JobDegree[];
  conditions?: JobCondition[];
  responsibilities?: JobResponsibility[];
  skills?: JobSkill[];
  requiredAttachments?: JobRequiredAttachment[];
  tabReviewNotes?: JobTabReviewNoteResponse[];
}

