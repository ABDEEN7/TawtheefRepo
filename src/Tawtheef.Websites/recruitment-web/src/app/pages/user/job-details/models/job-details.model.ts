import {DropdownOptionVM} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';
import {JobConditionResponse} from './job-condition-response.model';
import {JobDegreeResponse} from './job-degree-response.model';
import {JobRequiredAttachmentResponse} from './job-required-attachment-response.model';
import {JobResponsibilityResponse} from './job-responsibility-response.model';
import {JobSkillResponse} from './job-skill-response.model';
import {JobSpecializationResponse} from './job-specialization-response.model';

export interface JobDetailsModel {
  id: GUID;
  title?: string;
  numberOfVacancies: number;
  closingDate: Date;
  minimumAge: number;
  maximumAge: number;
  yearsOfExperience: number;
  createdDate: Date;
  modifiedDate?: Date;
  publishAt?: Date;
  cancelledAt?: Date;
  overView?: string;
  benefits?: string;
  qualificationDescription?: string;
  sector?: DropdownOptionVM;
  management?: DropdownOptionVM;
  department?: DropdownOptionVM;
  jobCategory?: DropdownOptionVM;
  gender?: DropdownOptionVM;
  workLocation?: DropdownOptionVM;
  major?: DropdownOptionVM;
  subMajor?: DropdownOptionVM;
  workType?: DropdownOptionVM;
  jobStatus?: DropdownOptionVM;
  degrees?: JobDegreeResponse[];
  conditions?: JobConditionResponse[];
  skills?: JobSkillResponse[];
  responsibilities?: JobResponsibilityResponse[];
  requiredAttachments?: JobRequiredAttachmentResponse[];
  jobSpecializations?: JobSpecializationResponse[];
  
  invitationStatusId?: GUID;
  invitationStatus?: DropdownOptionVM;
}
