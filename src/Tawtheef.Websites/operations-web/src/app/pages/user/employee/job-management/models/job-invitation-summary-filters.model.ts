import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';

export interface JobSummaryCriteria {
  year?: number | null;
  jobCategoryId?: string | null;
  departmentId?: string | null;
  jobStatusId?: string | null;
  search?: string | null;
}

export interface JobSummaryFilters extends JobSummaryCriteria, PaginatedRequest {}

export interface JobSummaryExportRequest extends JobSummaryCriteria {
  sortBy?: string;
  sortDirection?: string;
}
