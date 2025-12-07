import {PaginatedRequest} from '../../../../core/models/paginated-request.model';

export interface CandidateInvitationFilters extends PaginatedRequest {
  jobCategoryId?: string | null;
  departmentId?: string | null;
  invitationStatusId?: string | null;
  jobTitle?: string | null;
}
