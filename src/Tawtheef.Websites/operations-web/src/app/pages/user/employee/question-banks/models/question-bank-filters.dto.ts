import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';

export interface QuestionBankFilters extends PaginatedRequest {
  search?: string;
  questionBankTypeId?: string;
  managementId?: string;
  jobTitleId?: string;
  stageId?: string;
  isActive?: boolean;
}
