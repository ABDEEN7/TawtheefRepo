import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';

export interface QuestionBankRequestFilters extends PaginatedRequest {
  search?: string; requestTypeId?: string; questionBankTypeId?: string;
  statusId?: string; managementId?: string; jobTitleId?: string;
}
export interface QuestionBankRequestListItem {
  id: string; questionBankId: string; requestTypeId: string; requestTypeNameAr: string; requestTypeNameEn: string;
  statusId: string; statusNameAr: string; statusNameEn: string; questionBankTypeId: string;
  questionBankTypeNameAr: string; questionBankTypeNameEn: string; managementId?: string;
  managementNameAr?: string; managementNameEn?: string; jobTitleId?: string; jobTitleNameAr?: string;
  jobTitleNameEn?: string; submittedById: string; submittedByNameAr?: string; submittedByNameEn?: string;
  submittedAt: string; currentReviewRound: number;
}
export interface CreateQuestionBankRequest {
  questionBankTypeId: string; managementId: string | null; jobTitleId: string | null;
  stageId: null; reason: string | null;
}
export const QuestionBankTypeIds = {
  Specialized: 'a7f9646b-fcc3-4a00-944c-ceed82acd557',
  Skills: '686928db-b630-4689-b265-ae17eded73cf',
  Educational: '0ef8f0d9-02d7-4863-9bfe-e63355f3686e'
} as const;
