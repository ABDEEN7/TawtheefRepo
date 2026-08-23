import { InvitationSource } from '../../../../../core/enums/invitation-source.enum';

export type SortDirection = 'asc' | 'desc';

export interface JobInfoVM {
  jobId: string;
  jobName: string;
  departmentName: string;
  jobStatus: StatusVM;
  closingDate: string;
  currentBatchNumber: string | null;
}

export interface StatusVM {
  id: string;
  name: string;
  backendName: string;
}

export interface InviteRowVM {
  inviteId: string;
  source: InvitationSource;
  fullName: string;
  nationality: string;
  personalNumber: string;
  phone: string;
  status: StatusVM;
  batchNumber: string;
  sentDate: string;
  invitationExpiryDate: string;
  readDate?: string | null;
  appliedDate?: string | null;
  declinedDate?: string | null;
  expiredDate?: string | null;
  lastActivityDate?: string | null;
  hasAttachments: boolean;
}

export interface JobInvitesStatsVM {
  total: number;
  new: number;
  read: number;
  applied: number;
  declined: number;
  cancelled: number;
  expired: number;
  pendingAttachmentApproval: number;
  returnedAttachment: number;
}

export interface JobInvitesRowsFilters {
  jobId: string;
  statusId: string | null;
  search: string;
  batchNumber?: string | null;
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  sortDirection: SortDirection;
}

export interface InvitationAttachmentVM {
  id: string; // AttachmentId (InvitationAttachment.Id)
  jobRequiredAttachmentId: string;
  titleEn: string;
  titleAr: string;
  isMandatory: boolean;
  resourceId: string;
  resourceUrl: string;
  resourceName: string;
  isApproved: boolean;
  isReturned: boolean;
  reviewNote: string;
}
