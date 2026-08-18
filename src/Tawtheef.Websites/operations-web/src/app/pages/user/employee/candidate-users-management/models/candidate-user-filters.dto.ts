import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';

export interface CandidateUserFilters {
  pageNumber: number;
  pageSize: number;
  search?: string | null;
  isBlocked?: boolean | null;
  profileStatuses?: ProfileStatusNumber[] | null;
}
