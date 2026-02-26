import {PaginatedRequest} from '../../../../../core/models/paginated-request.model';
import {ReviewStatus} from '../../../employee/profile-managment/approval-list/models/profile-approval.models';

export interface SystemAdminLogFilters extends PaginatedRequest {
  userProfileId?: string | null;
  userId?: string | null;
  source?: string | null;
  actionType?: string | null;
  reviewStatus?: ReviewStatus | null;
  from?: string | null;
  to?: string | null;
  search?: string | null;
}
