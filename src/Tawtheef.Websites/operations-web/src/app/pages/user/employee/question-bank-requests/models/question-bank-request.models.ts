import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';

export interface QuestionBankRequestFilters extends PaginatedRequest {
  search?: string;
  requestTypeId?: string;
  questionBankTypeId?: string;
  statusId?: string;
  managementId?: string;
  jobTitleId?: string;
}
export interface QuestionBankRequestListItem {
  id: string;
  questionBankId: string;
  requestTypeId: string;
  requestTypeNameAr: string;
  requestTypeNameEn: string;
  statusId: string;
  statusNameAr: string;
  statusNameEn: string;
  questionBankTypeId: string;
  questionBankTypeNameAr: string;
  questionBankTypeNameEn: string;
  managementId?: string;
  managementNameAr?: string;
  managementNameEn?: string;
  jobTitleId?: string;
  jobTitleNameAr?: string;
  jobTitleNameEn?: string;
  submittedById: string;
  submittedByNameAr?: string;
  submittedByNameEn?: string;
  submittedAt: string;
  currentReviewRound: number;
  canReview: boolean;
}
export interface CreateQuestionBankRequest {
  questionBankTypeId: string;
  managementId: string | null;
  jobTitleId: string | null;
  stageId: null;
  reason: string | null;
}
export const QuestionBankTypeIds = {
  Specialized: 'a7f9646b-fcc3-4a00-944c-ceed82acd557',
  Skills: '686928db-b630-4689-b265-ae17eded73cf',
  Educational: '0ef8f0d9-02d7-4863-9bfe-e63355f3686e',
} as const;
export const QuestionBankRequestStatusIds = {
  PendingAssignment: '4b25e8db-92f1-4b4c-baab-756a5d1fd3f8',
  QuestionEntryInProgress: '66fbfd41-563e-4a12-a006-8a970285f62b',
  PendingReview: '97c0610c-a992-43e1-b9a1-bfdbcf5b34b0',
} as const;
export interface QuestionBankAssignment {
  id: string;
  employeeId: string;
  employeeNameAr?: string;
  employeeNameEn?: string;
  statusId: string;
  statusNameAr: string;
  statusNameEn: string;
  minimumQuestionCount: number;
  notes?: string;
  assignedAt: string;
  questionEntryStartedAt?: string;
  questionEntryCompletedAt?: string;
  lastReturnedForModificationAt?: string;
  lastModificationCompletedAt?: string;
  completedAt?: string;
}
export interface QuestionBankRequestDetails extends QuestionBankRequestListItem {
  reason?: string;
  isActive: boolean;
  canReview: boolean;
  pendingReviewItemCount: number;
  approvedItemCount: number;
  needsModificationItemCount: number;
  rejectedItemCount: number;
  assignments: QuestionBankAssignment[];
}
export interface EmployeeLookup {
  id: string;
  nameAr?: string;
  nameEn?: string;
  name?: string;
}
export interface AssignmentInput {
  employeeId: string;
  minimumQuestionCount: number;
  notes: string | null;
}
export interface AssignQuestionBankEmployees {
  requestId: string;
  assignments: AssignmentInput[];
}

export const QuestionReviewDecisionIds = {
  Approved: '73df4c1d-05a0-4bb4-9f19-5889a791aa27',
  NeedsModification: 'a29e6157-401f-49ac-ba46-98ecd8ca73dd',
  Rejected: 'a0ce4f21-955b-4290-bf88-fc1775a21410',
} as const;
export interface QuestionBankReviewOption {
  id: string;
  optionTextAr?: string;
  optionTextEn?: string;
  isCorrect: boolean;
  displayOrder: number;
}
export interface QuestionBankReviewItem {
  requestItemId: string;
  questionId: string;
  questionBankAssignmentId: string;
  employeeNameAr?: string;
  employeeNameEn?: string;
  changeTypeId: string;
  itemStatusId: string;
  currentProposedRevisionId: string;
  questionTypeId: string;
  questionTypeNameAr: string;
  questionTypeNameEn: string;
  difficultyLevelId: string;
  difficultyNameAr: string;
  difficultyNameEn: string;
  questionTextAr?: string;
  questionTextEn?: string;
  explanationAr?: string;
  explanationEn?: string;
  imageUrl?: string;
  options: QuestionBankReviewOption[];
}
export interface QuestionBankRequestReview {
  requestId: string;
  questionBankId: string;
  requestTypeId: string;
  requestTypeNameAr: string;
  requestTypeNameEn: string;
  requestStatusId: string;
  requestStatusNameAr: string;
  requestStatusNameEn: string;
  currentReviewRound: number;
  questionBankTypeId: string;
  questionBankTypeNameAr: string;
  questionBankTypeNameEn: string;
  managementNameAr?: string;
  managementNameEn?: string;
  jobTitleNameAr?: string;
  jobTitleNameEn?: string;
  items: QuestionBankReviewItem[];
}
export interface QuestionReviewInput {
  requestItemId: string;
  reviewedRevisionId: string;
  decisionId: string;
  reviewNote: string | null;
}
export interface SubmitQuestionBankReviewResult {
  changesRequired: boolean;
  issued: boolean;
  reviewRound: number;
}
