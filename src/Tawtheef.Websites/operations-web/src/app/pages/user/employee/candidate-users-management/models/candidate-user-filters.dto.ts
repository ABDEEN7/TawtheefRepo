import {
  ProfileStatus,
  ProfileStatusNumber,
} from '../../../../../core/enums/lookups.enum';

export enum CandidateUsersResultScope {
  Default = 'Default',
  AccessibleProfiles = 'AccessibleProfiles',
}

export interface CandidateUserFilters {
  pageNumber: number;
  pageSize: number;

  search?: string | null;
  isBlocked?: boolean | null;
  profileStatuses?: ProfileStatusNumber[] | null;

  profileStatus?: ProfileStatus;
  year?: number;

  scope: CandidateUsersResultScope;
}