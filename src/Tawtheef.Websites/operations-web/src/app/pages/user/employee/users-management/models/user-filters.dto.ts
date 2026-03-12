export interface UserFilters {
  pageNumber: number;
  pageSize: number;
  name?: string;
  email?: string;
  isBlocked?: boolean | null;
}
