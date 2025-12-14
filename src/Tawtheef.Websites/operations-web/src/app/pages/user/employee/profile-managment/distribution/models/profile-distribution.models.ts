import {ProfileStatusNumber} from '../../../../../../core/enums/lookups.enum';

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
  status: ProfileStatusNumber;
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
