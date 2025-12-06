import { dropdownOptionsModel } from "../../../shared/models/dropdown-options.model";
import { GUID } from "../../../shared/types/guid.type";
import { JobConditionResponse } from "./job-condition-response.model";
import { JobDegreeResponse } from "./job-degree-response.model";
import { JobQuotaResponse } from "./job-quota-response.model";
import { JobRequiredAttachmentResponse } from "./job-required-attachment-response.model";
import { JobResponsibilityResponse } from "./job-responsibility-response.model";
import { JobSkillResponse } from "./job-skill-response.model";


export interface JobResponse {
  id: GUID;
  
  titleAr: string;
  titleEn: string;
  numberOfVacancies: number;        
  closingDate: Date;
  minimumAge: number;
  maximumAge: number;
  yearsOfExperience: number;       
  
  createdDate: Date;
  modifiedDate?: Date;
  publishAt?: Date;
  cancelledAt?: Date;
  
  createdBy?: GUID;
  modifiedBy?: GUID;
  
  overviewAr?: string;
  overviewEn?: string;
  benefitsAr?: string;
  benefitsEn?: string;
  qualificationsDescriptionAr?: string;  
  qualificationsDescriptionEn?: string; 
  
  sector: dropdownOptionsModel;
  management: dropdownOptionsModel;
  department: dropdownOptionsModel;
  jobCategory: dropdownOptionsModel;
  gender?: dropdownOptionsModel;
  workLocation: dropdownOptionsModel;
  major: dropdownOptionsModel;
  subMajor?: dropdownOptionsModel;
  workType: dropdownOptionsModel;
  status: dropdownOptionsModel;
  
  quota?: JobQuotaResponse;
  degrees: JobDegreeResponse[];
  conditions: JobConditionResponse[];
  skills: JobSkillResponse[];
  responsibilities: JobResponsibilityResponse[];
  requiredAttachments: JobRequiredAttachmentResponse[];
}