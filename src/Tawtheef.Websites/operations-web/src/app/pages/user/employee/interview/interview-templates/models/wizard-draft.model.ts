import { CalculationMethod } from './enums';
import { TemplateVersionAxisModel, TemplateVersionCriterionModel } from './template-version.model';

export type CriterionDraftMode = 'bank' | 'custom';

export interface WizardCriterionDraft {
  localId: string;
  /** Existing InterviewTemplateEvaluationCriterion id when editing a version; null for a row added in this session. */
  id: string | null;
  mode: CriterionDraftMode;
  criterionId: string | null;
  nameAr: string;
  nameEn: string | null;
  descriptionAr: string | null;
  descriptionEn: string | null;
  maxScore: number;
  isRequired: boolean;
  notes: string | null;
  orderNo: number;
}

export interface WizardAxisDraft {
  localId: string;
  /** Existing InterviewTemplateEvaluationAxis id when editing a version; null for a row added in this session. */
  id: string | null;
  axisId: string;
  maxScore: number;
  qualificationScore: number | null;
  orderNo: number;
  criteria: WizardCriterionDraft[];
}

export interface WizardInfoDraft {
  titleAr: string;
  titleEn: string;
  organizationScopeId: string;
  jobTitleId: string;
  departmentId: string;
  finalScore: number;
  qualificationScore: number | null;
  calculationMethod: CalculationMethod;
}

export type WizardMode = 'new-template' | 'new-version' | 'edit-version';

export function emptyWizardInfoDraft(): WizardInfoDraft {
  return {
    titleAr: '',
    titleEn: '',
    organizationScopeId: '',
    jobTitleId: '',
    departmentId: '',
    finalScore: 100,
    qualificationScore: null,
    calculationMethod: 1,
  };
}

export function newWizardAxisDraft(axisId = ''): WizardAxisDraft {
  return {
    localId: crypto.randomUUID(),
    id: null,
    axisId,
    maxScore: 0,
    qualificationScore: null,
    orderNo: 1,
    criteria: [],
  };
}

export function newWizardCriterionDraft(): WizardCriterionDraft {
  return {
    localId: crypto.randomUUID(),
    id: null,
    mode: 'custom',
    criterionId: null,
    nameAr: '',
    nameEn: null,
    descriptionAr: null,
    descriptionEn: null,
    maxScore: 0,
    isRequired: true,
    notes: null,
    orderNo: 1,
  };
}

export function criterionDraftFromExisting(criterion: TemplateVersionCriterionModel): WizardCriterionDraft {
  const isBank = !!criterion.interviewEvaluationCriterionId;
  return {
    localId: crypto.randomUUID(),
    id: criterion.id,
    mode: isBank ? 'bank' : 'custom',
    criterionId: criterion.interviewEvaluationCriterionId ?? null,
    nameAr: isBank ? '' : criterion.nameAr || '',
    nameEn: isBank ? null : criterion.nameEn ?? null,
    descriptionAr: isBank ? null : criterion.descriptionAr ?? null,
    descriptionEn: isBank ? null : criterion.descriptionEn ?? null,
    maxScore: criterion.maxScore,
    isRequired: criterion.isRequired,
    notes: criterion.notes ?? null,
    orderNo: criterion.orderNo,
  };
}

export function axisDraftFromExisting(axis: TemplateVersionAxisModel): WizardAxisDraft {
  return {
    localId: crypto.randomUUID(),
    id: axis.id,
    axisId: axis.interviewEvaluationAxisId,
    maxScore: axis.maxScore,
    qualificationScore: axis.qualificationScore ?? null,
    orderNo: axis.orderNo,
    criteria: axis.criteria.map(criterionDraftFromExisting),
  };
}
