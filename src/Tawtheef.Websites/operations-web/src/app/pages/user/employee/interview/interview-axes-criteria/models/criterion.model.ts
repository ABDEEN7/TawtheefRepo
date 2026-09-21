export interface CriterionModel {
  id: string;
  interviewEvaluationAxisId: string;
  nameAr: string;
  nameEn?: string | null;
  descriptionAr?: string | null;
  descriptionEn?: string | null;
  isActive: boolean;
}
