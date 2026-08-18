import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';
import { ReviewStatus } from '../../profile-managment/approval-list/models/profile-approval.models';
import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';

export interface ProfileLogFilters extends PaginatedRequest {
  userProfileId?: string | null;
  userId?: string | null;
  source?: string | null;
  actionType?: string | null;
  reviewStatus?: ReviewStatus | null;
  from?: string | null;
  to?: string | null;
  search?: string | null;
  candidateSearch?: string | null;
  notesSearch?: string | null;
  sections?: string[] | null;
  profileStatuses?: ProfileStatusNumber[] | null;
}
