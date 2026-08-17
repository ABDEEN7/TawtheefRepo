export interface OperationsDashboardFilters {
  year?: number;
  departmentId?: string;
  employeeId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDirection?: string;
}

export interface OperationsDashboardFilterSnapshot {
  year: number;
  fromDateUtc?: string;
  toDateUtc?: string;
  departmentId?: string;
  employeeId?: string;
  status?: string;
}
