export interface QuestionBankListItemDto {
  id: string;
  questionBankTypeId: string;
  questionBankTypeNameAr: string;
  questionBankTypeNameEn: string;
  managementId?: string | null;
  managementNameAr?: string | null;
  managementNameEn?: string | null;
  jobTitleId?: string | null;
  jobTitleNameAr?: string | null;
  jobTitleNameEn?: string | null;
  stageId?: string | null;
  stageNameAr?: string | null;
  stageNameEn?: string | null;
  currentApprovedVersionId?: string | null;
  currentVersionNo?: number | null;
  isActive: boolean;
  createdDate: string;
  updatedDate?: string | null;
}
