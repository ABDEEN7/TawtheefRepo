export enum ProfileFileStatus {
  Submitted = 1,
  UnderReview = 2,
  NeedsChanges = 3,
  Approved = 4,
  Rejected = 5,
  Cancelled = 6,
}

export enum EmployeeAvailability {
  Available = 1,
  OnLeave = 2,
  Suspended = 3,
  Inactive = 4,
}

export interface DistributionFile {
  profileId: string;
  candidateName: string;
  specialization: string;
  targetEntity: string;
  status: ProfileFileStatus;
  assignedEmployeeId?: string;
  assignedEmployeeName?: string;
  submittedAtUtc: string;
}

export interface DistributionEmployee {
  employeeId: string;
  name: string;
  totalAssigned: number;
  completed: number;
  inReview: number;
  isActive: boolean;
  availability: EmployeeAvailability;
}

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

export interface DistributionResult {
  assignedCount: number;
  employees: DistributionEmployee[];
  profiles: DistributionFile[];
}
