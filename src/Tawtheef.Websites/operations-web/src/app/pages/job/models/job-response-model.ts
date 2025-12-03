import { Lookups } from "../../../core/models/lookups.model";
import { GUID } from "../../../shared/types/guid.type";
import { JobConditionResponseDto } from "./job-condition-response.model";
import { JobDegreeResponseDto } from "./job-degree-response.model";
import { JobQuotaResponse } from "./job-quota-response.model";
import { JobRequiredAttachmentResponseDto } from "./job-required-attachment-response.model";
import { JobResponsibilityResponseDto } from "./job-responsibility-response.model";
import { JobSkillResponseDto } from "./job-skill-response.model";

export interface JobResponseDto {
  id: GUID;

  // Metadata
  createdBy?: string;
  createdDate: string;
  modifiedBy?: string;
  modifiedDate?: string;

  // Basic Job Information
  title: string;
  vacancies: number;
  deadline: string;
  description: string;
  benefits: string;
  overview: string;
  qualificationsDescription?: string;
  publishAt?: string;

  // Requirements
  minimumExperienceYears: number;
  minimumAge: number;
  maximumAge: number;

  // Lookups
  sector: Lookups;
  management: Lookups;
  department: Lookups;
  jobCategory: Lookups;
  gender?: Lookups;
  workLocation: Lookups;
  major: Lookups;
  subMajor?: Lookups;
  workType: Lookups;
  status: Lookups;

  // Quota
  quota: JobQuotaResponse;

  // Collections
  degrees: JobDegreeResponseDto[];
  conditions: JobConditionResponseDto[];
  skills: JobSkillResponseDto[];
  responsibilities: JobResponsibilityResponseDto[];
  requiredAttachments: JobRequiredAttachmentResponseDto[];
}