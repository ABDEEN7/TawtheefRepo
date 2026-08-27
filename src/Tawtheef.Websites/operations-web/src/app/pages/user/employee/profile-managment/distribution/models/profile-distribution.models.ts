import {ProfileStatusNumber} from '../../../../../../core/enums/lookups.enum';
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
  searchText: string;
  totalAssigned: number;
  completed: number;
  inReview: number;
  isActive: boolean;
  availability: EmployeeAvailability;
}

export interface DistributionResult {
  assignedCount: number;
  employees: DistributionEmployee[];
  profiles: DistributionFile[];
}
