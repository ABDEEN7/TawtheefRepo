import {ProfileStatusNumber} from '../../../../../../core/enums/lookups.enum';
import {PaginatedRequest} from '../../../../../../core/models/paginated-request.model';

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
  hasOtherSpecialization: boolean;
  hasOtherUniversity: boolean;
  isMinisterOfficeCandidate: boolean;
  submittedAtUtc: string;
}

export interface DistributionEmployee {
  employeeId: string;
  name: string;
  email: string;
  totalAssigned: number;
  completed: number;
  inReview: number;
  isActive: boolean;
  availability: EmployeeAvailability;
}

export type DistributionEmployeeLookup = Pick<
  DistributionEmployee,
  'employeeId' | 'name' | 'email' | 'isActive' | 'availability'
>;

export interface DistributionEmployeeFilters extends PaginatedRequest {
  searchTerm?: string;
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

export interface DistributionAppliedFilters {
  searchTerm: string | null;
  statuses: ProfileStatusNumber[];
  assignedEmployeeId: string | null;
  targetEntityId: string | null;
  candidateTypeIds: string[];
  hasOtherSpecialization: boolean | null;
  hasOtherUniversity: boolean | null;
  isQatarGraduate: boolean;
  degreeIds: string[];
  pageNumber: number;
  pageSize: number;
  sortBy: 'CreatedDate';
  sortDirection: 'asc' | 'desc';
}

export type DistributionAdvancedFilters = Omit<DistributionAppliedFilters,
  'searchTerm' | 'statuses' | 'pageNumber' | 'pageSize' | 'sortBy' | 'sortDirection'>;

export interface DistributionProfilesFilters extends PaginatedRequest {
  searchTerm?: string;
  statuses?: ProfileStatusNumber[];
  assignedEmployeeId?: string;
  targetEntityId?: string;
  candidateTypeIds?: string[];
  hasOtherSpecialization?: boolean;
  hasOtherUniversity?: boolean;
  isQatarGraduate?: boolean;
  degreeIds?: string[];
}
