import { ReviewStatus } from '../../profile-managment/approval-list/models/profile-approval.models';

export interface SystemAdminLogDto {
  id: string;
  userProfileId: string;
  source: string;
  logType?: string | null;
  actionType: string;
  section?: string | null;
  notes?: string | null;
  userId?: string | null;
  userName?: string | null;
  userProfileOwnerName?: string | null;
  entityId?: string | null;
  attachmentId?: string | null;
  reviewStatus?: ReviewStatus | null;
  createdDate: string;
}
