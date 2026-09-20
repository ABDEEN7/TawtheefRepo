import { TemplateVersionStatus } from './enums';

export interface TemplateModel {
  id: string;
  titleAr: string;
  titleEn?: string | null;
  organizationScopeId?: string | null;
  organizationScopeNameAr?: string | null;
  organizationScopeNameEn?: string | null;
  jobTitleId?: string | null;
  jobTitleNameAr?: string | null;
  jobTitleNameEn?: string | null;
  departmentId?: string | null;
  departmentNameAr?: string | null;
  departmentNameEn?: string | null;
  isActive: boolean;
  latestVersionNo?: number | null;
  latestVersionStatus?: TemplateVersionStatus | null;
  latestVersionFinalScore?: number | null;
  latestVersionQualificationScore?: number | null;
}
