export interface AxisBankOption {
  id: string;
  nameAr: string;
  nameEn?: string | null;
}

export interface CriterionBankOption {
  id: string;
  interviewEvaluationAxisId: string;
  nameAr: string;
  nameEn?: string | null;
  descriptionAr?: string | null;
  descriptionEn?: string | null;
}
