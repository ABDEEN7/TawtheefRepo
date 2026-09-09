export interface ExamLookupsDto {
  categories: ExamLookupItemDto[];
  interruptionPolicies: ExamLookupItemDto[];
  specializedCategoryId: string;
}

export interface ExamLookupItemDto {
  id: string;
  nameAr: string;
  nameEn: string;
}