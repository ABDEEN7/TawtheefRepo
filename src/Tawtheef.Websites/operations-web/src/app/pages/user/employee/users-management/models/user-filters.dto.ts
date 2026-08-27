export interface UserFilters {
  pageNumber: number;
  pageSize: number;
  search?: string | null;
  isBlocked?: boolean | null;
}
