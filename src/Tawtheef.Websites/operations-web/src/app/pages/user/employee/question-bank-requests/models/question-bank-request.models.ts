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
export const QuestionBankRequestStatusIds = {
  PendingAssignment: '4b25e8db-92f1-4b4c-baab-756a5d1fd3f8',
  QuestionEntryInProgress: '66fbfd41-563e-4a12-a006-8a970285f62b'
} as const;
export interface QuestionBankAssignment {
  id: string; employeeId: string; employeeNameAr?: string; employeeNameEn?: string;
  statusId: string; statusNameAr: string; statusNameEn: string; minimumQuestionCount: number;
  notes?: string; assignedAt: string; questionEntryStartedAt?: string; questionEntryCompletedAt?: string;
  lastReturnedForModificationAt?: string; lastModificationCompletedAt?: string; completedAt?: string;
}
export interface QuestionBankRequestDetails extends QuestionBankRequestListItem {
  reason?: string; isActive: boolean; assignments: QuestionBankAssignment[];
}
export interface EmployeeLookup { id: string; nameAr?: string; nameEn?: string; name?: string; }
export interface AssignmentInput { employeeId: string; minimumQuestionCount: number; notes: string | null; }
export interface AssignQuestionBankEmployees { requestId: string; assignments: AssignmentInput[]; }
