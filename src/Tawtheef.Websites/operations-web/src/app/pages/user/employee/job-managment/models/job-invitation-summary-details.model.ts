export type SortDirection = 'asc' | 'desc';

export interface LookupOption {
  id: string;
  name: string;
}

export interface JobInfoVM {
  jobId: string;
  jobName: string;
  departmentName: string;
  jobStatus: StatusVM;
  currentBatchNumber: string | null;
}

export interface StatusVM {
  id: string;
  name: string;
  backendName: string;
}

export interface InviteRowVM {
  inviteId: string;
  fullName: string;
  nationality: string;
  personalNumber: string;
  phone: string;
  status: StatusVM;
  batchNumber: string;
  sentDate: string;
  readDate?: string | null;
  appliedDate?: string | null;
  declinedDate?: string | null;
  expiredDate?: string | null;
}

export interface PaginationMetadata {
  totalCount: number;
  totalPages: number;
}

export interface JobInvitesStatsVM {
  total: number;
  new: number;
  read: number;
  applied: number;
  declined: number;
  cancelled: number;
  expired: number;
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
