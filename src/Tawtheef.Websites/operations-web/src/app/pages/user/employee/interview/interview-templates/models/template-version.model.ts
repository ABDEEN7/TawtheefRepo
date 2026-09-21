import { CalculationMethod, TemplateVersionStatus } from './enums';

export interface TemplateVersionModel {
  id: string;
  interviewTemplateId: string;
  versionNo: number;
  finalScore: number;
  qualificationScore?: number | null;
  calculationMethod: CalculationMethod;
  status: TemplateVersionStatus;
  isLocked: boolean;
  effectiveFrom?: string | null;
  approvedById?: string | null;
  approvedAt?: string | null;
  decisionNotes?: string | null;
}

export interface TemplateVersionCriterionModel {
  id: string;
  interviewTemplateEvaluationAxisId: string;
  interviewEvaluationCriterionId?: string | null;
  nameAr?: string | null;
  nameEn?: string | null;
  descriptionAr?: string | null;
  descriptionEn?: string | null;
  maxScore: number;
  isRequired: boolean;
  orderNo: number;
  notes?: string | null;
}

export interface TemplateVersionAxisModel {
  id: string;
  interviewTemplateVersionId: string;
  interviewEvaluationAxisId: string;
  axisNameAr: string;
  axisNameEn?: string | null;
  maxScore: number;
  qualificationScore?: number | null;
  orderNo: number;
  criteria: TemplateVersionCriterionModel[];
}

export interface TemplateVersionDetailsModel extends TemplateVersionModel {
  axes: TemplateVersionAxisModel[];
}
