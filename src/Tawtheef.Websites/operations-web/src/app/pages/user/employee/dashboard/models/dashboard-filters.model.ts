export interface OperationsDashboardFilters {
  fromDateUtc?: string;
  toDateUtc?: string;
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
  fromDateUtc?: string;
  toDateUtc?: string;
  departmentId?: string;
  employeeId?: string;
  status?: string;
}
