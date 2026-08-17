export interface ManualAssignRequest {
  employeeId: string;
  profileIds: string[];
}

export interface AutoAssignRequest {
  employeeIds: string[];
  profileIds?: string[];
  perEmployeeCount?: number | null;
}

export interface ReassignRequest {
  mode: 'manual' | 'auto';
  employeeId?: string;
  employeeIds?: string[];
  profileIds: string[];
  perEmployeeCount?: number | null;
}
