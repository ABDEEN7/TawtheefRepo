export type SortDirection = 'asc' | 'desc';

export interface LookupOption {
  id: string;
  name: string;
}

export interface JobInfoVM {
  jobId: string;
  jobName: string;
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
  phone: string;
  status: StatusVM;
  sentDate: string;
}

export interface PaginationMetadata {
  totalCount: number;
  totalPages: number;
}

export interface JobInvitesStatsVM {
  total: number;
  new: number;
  applied: number;
  declined: number;
  cancelled: number;
}

export interface JobInvitesRowsFilters {
  jobId: string;
  statusId: string | null;
  search: string;
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  sortDirection: SortDirection;
}
