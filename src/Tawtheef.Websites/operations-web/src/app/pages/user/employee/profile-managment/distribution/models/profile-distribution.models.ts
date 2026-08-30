import { ProfileStatusNumber } from '../../../../../../core/enums/lookups.enum';
import { PaginatedRequest } from '../../../../../../core/models/paginated-request.model';
import { EmployeeAvailability } from './profile-distribution.enums';

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
  searchText: string;
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

export interface DistributionResult {
  assignedCount: number;
  employees: DistributionEmployee[];
  profiles: DistributionFile[];
}
