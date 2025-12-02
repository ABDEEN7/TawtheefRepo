import {PaginatedRequest} from '../../core/models/paginated-request.model';

export interface JobInvitationSummary {
  jobId: string; // Guid
  jobName: string;
  departmentName: string;
  jobCategory: string;
  jobStatusBackendName: string;
  invitationCount: number;
  applicantsCount: number;
  refusedCount: number;
  notSeenCount: number;
  createDate: string;           // ISO date string
}

export interface JobSummaryFilters extends PaginatedRequest {
  jobCategoryId?: string | null;
  departmentId?: string | null;
  jobStatusId?: string | null;
}
