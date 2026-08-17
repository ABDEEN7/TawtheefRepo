import { ProfileStatusNumber } from '../../../../../../core/enums/lookups.enum';
import { PaginatedRequest } from '../../../../../../core/models/paginated-request.model';
import { DistributionAssignmentState } from './profile-distribution.enums';

export interface DistributionAppliedFilters {
  searchTerm: string | null;
  statuses: ProfileStatusNumber[];
  year: number | null;
  assignmentState: DistributionAssignmentState | null;
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
  'searchTerm' | 'statuses' | 'year' | 'pageNumber' | 'pageSize' | 'sortBy' | 'sortDirection'>;

export interface DistributionProfilesFilters extends PaginatedRequest {
  searchTerm?: string;
  statuses?: ProfileStatusNumber[];
  year?: number;
  assignmentState?: DistributionAssignmentState;
  assignedEmployeeId?: string;
  targetEntityId?: string;
  candidateTypeIds?: string[];
  hasOtherSpecialization?: boolean;
  hasOtherUniversity?: boolean;
  isQatarGraduate?: boolean;
  degreeIds?: string[];
}
