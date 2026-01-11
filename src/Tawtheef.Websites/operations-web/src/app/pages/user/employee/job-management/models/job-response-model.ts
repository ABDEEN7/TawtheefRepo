import { dropdownOptionsModel } from "../../../../../shared/models/dropdown-options.model";
import { GUID } from "../../../../../shared/types/guid.type";
import { JobConditionResponse } from "./job-condition-response.model";
import { JobDegreeResponse } from "./job-degree-response.model";
import { JobPointsResponse } from "./job-points-response";
import { JobRequiredAttachmentResponse } from "./job-required-attachment-response.model";
import { JobResponsibilityResponse } from "./job-responsibility-response.model";
import { JobSkillResponse } from "./job-skill-response.model";
import { JobTabReviewNoteResponse } from "./job-tab-review-note-response";


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

  overViewAr?: string;
  overViewEn?: string;
  benefitsAr?: string;
  benefitsEn?: string;
  qualificationDescriptionAr?: string;
  qualificationDescriptionEn?: string;
  approveNote?: string;
  rejectNote?:string;

  sector: dropdownOptionsModel;
  management: dropdownOptionsModel;
  department: dropdownOptionsModel;
  jobCategory: dropdownOptionsModel;
  gender?: dropdownOptionsModel;
  workLocation: dropdownOptionsModel;
  major: dropdownOptionsModel;
  subMajor?: dropdownOptionsModel;
  workType: dropdownOptionsModel;
  jobStatus: dropdownOptionsModel;

  jobPoints : JobPointsResponse;
  degrees: JobDegreeResponse[];
  conditions: JobConditionResponse[];
  skills: JobSkillResponse[];
  responsibilities: JobResponsibilityResponse[];
  requiredAttachments: JobRequiredAttachmentResponse[];
  tabReviewNotes?: JobTabReviewNoteResponse[];
}
