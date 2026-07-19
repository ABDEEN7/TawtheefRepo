import {PaginatedRequest} from '../../../../core/models/paginated-request.model';
import { GUID } from '../../../../shared/types/guid.type';

export interface CandidateInvitationFilters extends PaginatedRequest {
  jobCategoryId?: string | null;
  departmentId?: string | null;
  invitationStatusId?: string | null;
  jobTitle?: string | null;
}
