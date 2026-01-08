import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';
import {JobConditionResponse} from './job-condition-response.model';
import {JobDegreeResponse} from './job-degree-response.model';
import {JobRequiredAttachmentResponse} from './job-required-attachment-response.model';
import {JobResponsibilityResponse} from './job-responsibility-response.model';
import {JobSkillResponse} from './job-skill-response.model';

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
  sector?: dropdownOptionsModel;
  management?: dropdownOptionsModel;
  department?: dropdownOptionsModel;
  jobCategory?: dropdownOptionsModel;
  gender?: dropdownOptionsModel;
  workLocation?: dropdownOptionsModel;
  major?: dropdownOptionsModel;
  subMajor?: dropdownOptionsModel;
  workType?: dropdownOptionsModel;
  jobStatus?: dropdownOptionsModel;
  degrees?: JobDegreeResponse[];
  conditions?: JobConditionResponse[];
  skills?: JobSkillResponse[];
  responsibilities?: JobResponsibilityResponse[];
  requiredAttachments?: JobRequiredAttachmentResponse[];
}
